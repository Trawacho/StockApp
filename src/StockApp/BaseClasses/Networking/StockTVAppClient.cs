using NetMQ;
using NetMQ.Monitoring;
using NetMQ.Sockets;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StockApp.BaseClasses
{
    /// <summary>
    /// NetMQ-client to communication with StockTV
    /// </summary>
    public class StockTVAppClient
    {
        public event StockTVOnlineChangedEventHandler StockTVOnlineChanged;
        protected void RaisStockTVOnlineChanged()
        {
            var handler = StockTVOnlineChanged;
            handler?.Invoke(this, this.IsOnline);
        }

        private NetMQPoller Poller;
        private RequestSocket Socket;
        private NetMQMonitor Monitor;
        private readonly string IPAddress;
        private readonly int Port;
        private readonly string Identifier;
        private readonly NetMQQueue<StockTVCommand> sendQueue;

        #region Konstruktor

        public StockTVAppClient(string ip, int port, string identifier)
        {
            //Debug.WriteLine($"Create ZMQ client for {identifier}");
            this.IsOnline = false;
            this.IPAddress = ip;
            this.Port = port;
            this.Identifier = identifier;

            sendQueue = new NetMQQueue<StockTVCommand>();
        }

        #endregion
        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                if (_isOnline == value) return;
                _isOnline = value;
                RaisStockTVOnlineChanged();
            }
        }


        #region Functions

        public string GetConnectionString()
        {
            return $"tcp://{IPAddress}:{Port}";
        }

        public void Start()
        {
            if (Socket != null) return;
            
            Socket = new RequestSocket(GetConnectionString());
            Socket.Options.Identity = Encoding.UTF8.GetBytes(this.Identifier);
            Socket.SendReady += Socket_SendReady;

            Poller = new NetMQPoller() { Socket };

            Monitor = new NetMQMonitor(Socket, $"inproc://{Identifier}.inproc", SocketEvents.All);
            Monitor.EventReceived += Monitor_EventReceived;
            Monitor.StartAsync();

            Poller.RunAsync(Identifier);
        }

        public void Stop()
        {
            Socket.SendReady -= Socket_SendReady; Debug.WriteLine("socket unsubscribe event");
            Socket.Disconnect(GetConnectionString()); Debug.WriteLine("socket disconntet");

            Poller.Stop(); Debug.WriteLine("Poller stopped");
            Poller.Remove(Socket); Debug.WriteLine("Poller removes socket");

            Poller.Dispose(); Debug.WriteLine("Poller dispose");
            Socket.Dispose(); Debug.WriteLine("Socket dispose");
            Monitor.Dispose(); Debug.WriteLine("Monitor dispose");

            Poller = null;
            Socket = null;
            Monitor = null;
        }

        public void AddCommand(StockTVCommand command)
        {
            sendQueue.Enqueue(command);
        }

        #endregion



        private void Monitor_EventReceived(object sender, NetMQMonitorEventArgs e)
        {
            Debug.WriteLine($"MONITOR: {e.Address} ..  {e.SocketEvent}");
            this.IsOnline = e.SocketEvent == SocketEvents.Connected;
        }

        StockTVCommand callBackCommand;
        private void Socket_SendReady(object sender, NetMQSocketEventArgs e)
        {
            if (sendQueue.Count > 0)
            {
                Debug.WriteLine(Identifier + " Disconnect SendReady event");
                e.Socket.SendReady -= Socket_SendReady;

                if (sendQueue.TryDequeue(out callBackCommand, TimeSpan.FromSeconds(50)))
                {
                    Debug.WriteLine(Identifier + "Send dequeued command");
                    e.Socket.SendFrame(callBackCommand.CommandString(), false);

                    Debug.WriteLine(Identifier + " Connect ReceiveReady after sending");
                    e.Socket.ReceiveReady += Socket_ReceiveReady;
                }
            }
           
        }

        private void Socket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            Debug.WriteLine(Identifier + "Disconnect ReceiveReady");
            e.Socket.ReceiveReady -= Socket_ReceiveReady;
            
            var msg = new NetMQMessage();
            if (e.Socket.TryReceiveMultipartMessage(TimeSpan.FromSeconds(5), ref msg))
            {
                Debug.WriteLine("answer received");

                if (msg.FrameCount == 2)
                {
                    string topic = msg.First().ConvertToString();
                    if (topic.Equals("Settings") || topic.Equals("Result"))
                    {
                        Debug.WriteLine("Callback startetd");
                        callBackCommand.CallBackAction?.Invoke(msg.Last().ToByteArray());
                    }
                }
            }
            else
            {
                Debug.WriteLine($"No answer received. In: {e.Socket.HasIn} Out {e.Socket.HasOut}");
            }

            Debug.WriteLine(Identifier + "Connect SendReady Event after receiving");
            e.Socket.SendReady += Socket_SendReady;
        }
    }


}

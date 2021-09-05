using NetMQ;
using NetMQ.Sockets;
using System;

namespace StockApp.BaseClasses
{
    /// <summary>
    /// NetMQ Subscriberclient to receive Push-Notificatoins from StockTV
    /// </summary>
    public class StockTVSubscriberClient
    {
        public class ShimHandler : IShimHandler
        {
            private PairSocket shim;
            private NetMQPoller poller;
            private SubscriberSocket resultSubscriber;
            private SubscriberSocket aliveSubscriber;
            private readonly string resultTopicString = "ResultInfo";
            private readonly string aliveTopicString = "Alive";
            private readonly string connectionString;

            private Action<byte[]> resultAction;
            private Action aliveAction;

            public ShimHandler(string connectionString, Action<byte[]> resultAction, Action aliveAction)
            {
                this.connectionString = connectionString;
                this.resultAction = resultAction;
                this.aliveAction = aliveAction;
            }

            public void Initialise(object state)
            {
            }

            public void Run(PairSocket shim)
            {
                using (resultSubscriber = new SubscriberSocket())
                using (aliveSubscriber = new SubscriberSocket())
                {
                    resultSubscriber.Options.ReceiveHighWatermark = 1000;
                    resultSubscriber.ReceiveReady += ResultSubscriber_ReceiveReady;
                    resultSubscriber.Connect(connectionString);
                    resultSubscriber.Subscribe(resultTopicString);

                    aliveSubscriber.Options.ReceiveHighWatermark = 1000;
                    aliveSubscriber.ReceiveReady += AliveSubscriber_ReceiveReady;
                    aliveSubscriber.Connect(connectionString);
                    aliveSubscriber.Subscribe(aliveTopicString);

                    this.shim = shim;
                    shim.ReceiveReady += OnShimReady;
                    shim.SignalOK();
                    poller = new NetMQPoller { shim, resultSubscriber, aliveSubscriber };
                    poller.Run();
                }
            }

            private void AliveSubscriber_ReceiveReady(object sender, NetMQSocketEventArgs e)
            {
                if (e.Socket.ReceiveFrameString().Equals("Alive"))
                {
                    aliveAction?.Invoke();
                }
            }

            private void ResultSubscriber_ReceiveReady(object sender, NetMQSocketEventArgs e)
            {
                //Check TOPIC of Message and set Value if TOPIC fits
                if (resultSubscriber.ReceiveFrameString().Equals("ResultInfo"))
                {
                    resultAction.Invoke(resultSubscriber.ReceiveFrameBytes());
                }
            }

            private void OnShimReady(object sender, NetMQSocketEventArgs e)
            {
                string command = e.Socket.ReceiveFrameString();
                if (command == NetMQActor.EndShimMessage)
                {
                    poller.Stop();
                }
            }
        }


        public event NotifyStockTVResultChangedHandler StockTVResultChanged;
        protected void RaiseStockTVResultChanged()
        {
            var handler = StockTVResultChanged;
            handler?.Invoke(this, StockTVResult);
        }


        private NetMQActor actor;

        public StockTVSubscriberClient(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
            StockTVResult = new StockTVResult();
        }

        /// <summary>
        /// e.g. tcp://192.168.100.130:4748
        /// </summary>
        public string Address => String.Format("tcp://{0}:{1}", IP, Port);
        public string IP { get; private set; }
        public int Port { get; private set; }

        public StockTVResult StockTVResult { private set; get; }
        public DateTime LastAliveReceived { private set; get; }

        public void Start()
        {
            if (actor != null)
                return;
            actor = NetMQActor.Create(new ShimHandler(
                                        Address
                                        , (array) =>
                                        {
                                            if (this.StockTVResult.Update(new StockTVResult(array)))
                                            {
                                                RaiseStockTVResultChanged();
                                            }
                                        }
                                        , () => LastAliveReceived = DateTime.Now));
        }

        public void Stop()
        {
            if (actor != null)
            {
                actor.Dispose();
                actor = null;
            }
        }


    }
}

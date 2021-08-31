using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace StockApp.BaseClasses
{
    public delegate void NetworkServiceDataReceivedEventHandler(object sender, NetworkServiceDataReceivedEventArgs e);
    public delegate void NetworkServiceStateChangedEventHandler(object sender, NetworkServiceStateEventArgs e);
    public sealed class NetworkService
    {
        private static readonly Lazy<NetworkService> lazy =
        new Lazy<NetworkService>(() => new NetworkService());

        public static NetworkService Instance { get { return lazy.Value; } }

        private UdpClient udpClient;
        private UdpState state;

        public event NetworkServiceStateChangedEventHandler StartStopStateChanged;
        void OnStartStopStateChanged(bool isRunning)
        {
            StartStopStateChanged?.Invoke(this, new NetworkServiceStateEventArgs(isRunning));
        }

        public event NetworkServiceDataReceivedEventHandler DataReceived;
        void RaiseDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, new NetworkServiceDataReceivedEventArgs(data));
        }

        private NetworkService()
        {
                
        }


        public void Start()
        {
            if (udpClient == null)
            {
                udpClient = new UdpClient();
                udpClient.Client.ReceiveTimeout = 500;
                udpClient.EnableBroadcast = true;
                udpClient.Client.Blocking = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Settings.Instanze.BroadcastPort));

            }
            if (state == null)
            {
                state = new UdpState()
                {
                    udpClient = udpClient,
                    ipEndPoint = new IPEndPoint(0,0),
                };
            }

            ReceiveBroadcast();
            OnStartStopStateChanged(true);
        }

        public void Stop()
        {
            udpClient.Close();
            udpClient.Dispose();
            udpClient = null;
            state.udpClient = null;
            state = null;

            OnStartStopStateChanged(false);
        }

        public bool IsRunning()
        {
            return (udpClient != null);
        }

        private void ReceiveBroadcast()
        {
            state.result = state.udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((UdpState)ar.AsyncState).udpClient;
                IPEndPoint e = ((UdpState)ar.AsyncState).ipEndPoint;
                IAsyncResult r = ((UdpState)ar.AsyncState).result;

                byte[] receiveBytes = u?.EndReceive(ar, ref e);
                if (receiveBytes?.Length > 1)
                {
                    RaiseDataReceived(DeCompress(receiveBytes));
                }

                r = u?.BeginReceive(new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ReceiveCallback: {e.Message}");
            }
        }

        byte[] DeCompress(byte[] data)
        {
            if (data == null)
                return null;

            using MemoryStream input = new MemoryStream(data);
            using MemoryStream output = new MemoryStream();
            using (var datastream = new DeflateStream(input, CompressionMode.Decompress))
            {
                datastream.CopyTo(output);
            }

            return output.ToArray();
        }


        private class UdpState
        {
            public UdpClient udpClient;
            public IPEndPoint ipEndPoint;
            public IAsyncResult result;
        }
    }
}

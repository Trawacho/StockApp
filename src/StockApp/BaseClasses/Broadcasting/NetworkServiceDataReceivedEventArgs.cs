using System;

namespace StockApp.BaseClasses
{
    public class NetworkServiceDataReceivedEventArgs : EventArgs
    {
        private readonly byte[] Data;
        public NetworkServiceDataReceivedEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public NetworkTelegram NetworkTelegram => new NetworkTelegram(Data);

    }
}

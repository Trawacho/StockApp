using System;

namespace StockApp.BaseClasses
{
    public class NetworkServiceDataReceivedEventArgs: EventArgs
    {
        public byte[] Data { get; set; }
        public NetworkServiceDataReceivedEventArgs(byte[] data)
        {
            this.Data = data;
        }
    }
}

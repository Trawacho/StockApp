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

        public int BahnNummer()
        {
            return Convert.ToInt32(Data[0]);
        }

        public int SpielGruppe()
        {
            return Data.Length % 2 == 0
                ? Convert.ToInt32(Data[1])
                : 0;
        }
    }
}

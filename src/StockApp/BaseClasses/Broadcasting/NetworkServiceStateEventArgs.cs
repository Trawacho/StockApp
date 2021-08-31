using System;

namespace StockApp.BaseClasses
{
    public class NetworkServiceStateEventArgs : EventArgs
    {
        public bool IsNetworkServiceOnline { get; set; }
        public NetworkServiceStateEventArgs(bool state)
        {
            this.IsNetworkServiceOnline = state;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.BaseClasses
{
    public class NetworkServiceEventArgs : EventArgs
    {
        public bool IsNetworkServiceOnline { get; set; }
        public NetworkServiceEventArgs(bool state)
        {
            this.IsNetworkServiceOnline = state;
        }
    }

}

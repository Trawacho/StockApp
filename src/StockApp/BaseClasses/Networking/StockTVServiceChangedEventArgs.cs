using System;

namespace StockApp.BaseClasses
{

    public class StockTVServiceChangedEventArgs: EventArgs
    {
        public StockTVServiceChangedEventArgs(StockTVService service)
        {
            this.Service = service;
        }

        public StockTVService Service { private set; get; }
    }
}
using System;

namespace StockApp.BaseClasses
{

    public class StockTVResultChangedEventArgs : EventArgs
    {
        public StockTVResultChangedEventArgs()
        {

        }

        public StockTVResultChangedEventArgs(StockTVResult tVResult) : this()
        {
            TVResult = tVResult;
        }

        public StockTVResult TVResult { get; private set; }
    }
}
using System;

namespace StockApp.BaseClasses
{

    public class StockTVGameResult
    {
        public StockTVGameResult(byte a, byte b)
        {
            ValueA = Convert.ToByte(a);
            ValueB = Convert.ToByte(b);
        }
        public byte GameNumber { get; set; }
        public byte ValueA { get; private set; }
        public byte ValueB { get; private set; }
        public override string ToString()
        {
            return $"{ValueA}:{ValueB}";
        }
    }
}

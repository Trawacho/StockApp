using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.BaseClasses
{
    public class StockTVResult : IEquatable<StockTVResult>
    {
        public StockTVResult()
        {
            Results = new List<StockTVGameResult>();
            Bahn = -1;
        }

        public StockTVResult(byte[] array) : this()
        {
            SetValues(array);
        }

        public StockTVResult(StockTVResult stockTVResult):this()
        {
            Update(stockTVResult);
        }

        private void SetValues(byte[] array)
        {
            byte b = Convert.ToByte(array[0]);
            if (Bahn != b) this.Bahn = b;

            var parts = array.Skip(1).Split(2);
            byte i = 1;
            this.Results.Clear();
            foreach (var item in parts)
            {
                this.Results.Add(new StockTVGameResult(item.First(), item.Last()) { GameNumber = i });
                i++;
            }
        }

        public bool Update(StockTVResult result)
        {
            if (this.Equals(result)) return false;

            this.Bahn = result.Bahn;
            this.Results.Clear();
            this.Results.AddRange(result.Results);
            return true;
        }

        public bool Equals(StockTVResult x)
        {
            if (x.Bahn == this.Bahn && x.Results.Count == this.Results.Count)
            {
                for (int i = 0; i < x.Results.Count; i++)
                {
                    if (!(x.Results[i].GameNumber == this.Results[i].GameNumber
                        && x.Results[i].ValueA == this.Results[i].ValueA
                        && x.Results[i].ValueB == this.Results[i].ValueB))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }



        public int Bahn { set; get; }

        public List<StockTVGameResult> Results { private set; get; }

        public override string ToString()
        {
            return $"Bahn: {Bahn} | Spiele:{Results.Count} | {string.Join("-", Results.Select(x=> String.Format("{0}:{1}",x.ValueA,x.ValueB)))}";
        }
    }
}

namespace StockApp.BaseClasses
{
    public class Startgebuehr : TBaseClass
    {
        /// <summary>
        /// Wert in Zahlen
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Wert in worten
        /// </summary>
        public string Verbal { get; set; }

        public Startgebuehr(double value, string verbal) : this()
        {
            this.Value = value;
            this.Verbal = verbal;
        }

        public Startgebuehr()
        {

        }
    }
}

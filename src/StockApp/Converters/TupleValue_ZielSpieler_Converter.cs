using StockApp.BaseClasses.Zielschiessen;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StockApp.Converters
{
    internal class TupleValue_ZielSpieler_Converter : IValueConverter
    {
        /// <summary>
        /// Returns a string from given ValueTuple as <int,Team> dependings on parameter
        /// 
        /// </summary>
        /// <param name="value">ValueTuple<int, Team></param>
        /// <param name="targetType">string</param>
        /// <param name="parameter">Name, Platzierung, Punkte, Details, Verein, Nation</param>
        /// <param name="culture">unused</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValueTuple<int, Teilnehmer, bool> t)
            {
                string p = (string)parameter;
                if (String.Equals("Name", p))
                {
                    return t.Item2.Name;
                }
                if (String.Equals("Platzierung", p))
                {
                    return t.Item1.ToString();
                }
                if (String.Equals("Verein", p))
                {
                    return t.Item2.Vereinsname;
                }
                if (String.Equals("Nation", p))
                {
                    return t.Item2.Nation;
                }
                if (String.Equals("Punkte", p))
                {
                    return t.Item2.GesamtPunkte.ToString();
                }
                if (String.Equals("Details", p))
                {
                    string s = "";
                    foreach (var w in t.Item2.Wertungen.OrderBy(o => o.Nummer))
                    {
                        s += $"{w.GesamtPunkte} >({w.PunkteMassenMitte}-{w.PunkteSchuesse}-{w.PunkteMassenSeitlich}-{w.PunkteKombinieren}) ";
                    }
                    return s;
                }

            }

            return "not def";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

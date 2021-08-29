using System.ComponentModel;
using System.Linq;

namespace StockApp.BaseClasses.Zielschiessen
{
    public enum Disziplinart
    {
        [Description("Massen Mitte")]
        MassenMitte = 1,

        [Description("Schiessen")]
        Schiessen = 2,

        [Description("Massen Seite")]
        MasseSeite = 3,

        [Description("Kombinieren")]
        Komibinieren = 4
    }

    public class Disziplin : TBaseClass
    {
        private readonly int[] versuche;

        /// <summary>
        /// Standard Konstruktor
        /// </summary>
        /// <param name="art"></param>
        public Disziplin(Disziplinart art)
        {
            versuche = new int[6];
            versuche[0] = -1;
            versuche[1] = -1;
            versuche[2] = -1;
            versuche[3] = -1;
            versuche[4] = -1;
            versuche[5] = -1;

            this.Disziplinart = art;
        }


        #region Properties

        public int Versuch1
        {
            get => versuche[0];
            set => AddVersuch(nameof(Versuch1), 0, value);
        }
        public int Versuch2
        {
            get => versuche[1];
            set => AddVersuch(nameof(Versuch2), 1, value);
        }
        public int Versuch3
        {
            get => versuche[2];
            set => AddVersuch(nameof(Versuch3), 2, value);
        }
        public int Versuch4
        {
            get => versuche[3];
            set => AddVersuch(nameof(Versuch4), 3, value);
        }
        public int Versuch5
        {
            get => versuche[4];
            set => AddVersuch(nameof(Versuch5), 4, value);
        }
        public int Versuch6
        {
            get => versuche[5];
            set => AddVersuch(nameof(Versuch6), 5, value);
        }



        /// <summary>
        /// Art der Disziplin dieser 6 Versuche
        /// </summary>
        public Disziplinart Disziplinart { get; }


        /// <summary>
        /// Summe aller Versuche die >= 0 sind
        /// </summary>
        public int Summe
        {
            get
            {
                return versuche.Where(x => x >= 0).Sum();
            }
        }

        /// <summary>
        /// Bzeichnung der Disziplin
        /// </summary>
        public string Name
        {
            get
            {
                return EnumUtil.GetEnumDescription(Disziplinart);
            }
        }

        #endregion

        #region Funktionen

        #region Public

        /// <summary>
        /// Jeder Versuch wird auf -1 gesetzt
        /// </summary>
        internal void Reset()
        {
            Versuch1 = -1;
            Versuch2 = -1;
            Versuch3 = -1;
            Versuch4 = -1;
            Versuch5 = -1;
            Versuch6 = -1;
        }

        /// <summary>
        /// Anzahl der Versuche die eingegeben wurden ( 0 bis 6 )
        /// </summary>
        /// <returns></returns>
        internal int VersucheCount()
        {
            return versuche.Where(v => v >= 0).Count();
        }

        #endregion

        #region Private

        /// <summary>
        /// Setzt den Wert in das Array der Versuche. Der Wert wird validiert.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void AddVersuch(string propertyName, int index, int value)
        {
            if (value == -1)
            {
                versuche[index] = value;
            }
            else if (Disziplinart == Disziplinart.Schiessen && IsSchussValue(value))
            {
                versuche[index] = value;
            }
            else if (Disziplinart != Disziplinart.Schiessen && IsMassValue(value))
            {
                versuche[index] = value;
            }

            RaisePropertyChanged(propertyName);
            RaisePropertyChanged(nameof(Summe));
        }


        /// <summary>
        /// Prüft, ob der Wert in einem Schuss-Versuch gültig ist
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsSchussValue(int value)
        {
            return value switch
            {
                0 or 2 or 5 or 10 => true,
                _ => false,
            };
        }

        /// <summary>
        /// Prüft, ob der Wert in einem Mass oder Komibinier-Versuch gültig ist
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsMassValue(int value)
        {
            return value switch
            {
                0 or 2 or 4 or 6 or 8 or 10 => true,
                _ => false,
            };
        }
        #endregion

        #endregion

    }

}

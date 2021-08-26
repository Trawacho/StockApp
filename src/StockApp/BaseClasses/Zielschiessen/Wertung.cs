using System.Collections.Generic;
using System.Linq;

namespace StockApp.BaseClasses.Zielschiessen
{
    public class Wertung : TBaseClass
    {
        private void Disziplin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (var item in typeof(Wertung).GetProperties())
            {
                RaisePropertyChanged(item.Name);
            }
        }


        private readonly List<Disziplin> disziplinen;
        private int _nummer;
        private bool _isOnline;

        /// <summary>
        /// Default-Konstruktor
        /// </summary>
        public Wertung()
        {
            var massenMitte = new Disziplin(Disziplinart.MassenMitte);
            massenMitte.PropertyChanged += Disziplin_PropertyChanged;

            var schiessen = new Disziplin(Disziplinart.Schiessen);
            schiessen.PropertyChanged += Disziplin_PropertyChanged;

            var massenSeite = new Disziplin(Disziplinart.MasseSeite);
            massenSeite.PropertyChanged += Disziplin_PropertyChanged;

            var kombinieren = new Disziplin(Disziplinart.Komibinieren);
            kombinieren.PropertyChanged += Disziplin_PropertyChanged;

            disziplinen = new List<Disziplin> { massenMitte, schiessen, massenSeite, kombinieren };

            _isOnline = false;
        }

        /// <summary>
        /// Nummer der Wertung
        /// </summary>
        public int Nummer
        {
            get => _nummer;
            set
            {
                if (_nummer == value)
                    return;
                _nummer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Sammlung der 4 Disziplinen
        /// </summary>
        public IEnumerable<Disziplin> Disziplinen { get => disziplinen; }


        /// <summary>
        /// TRUE wenn diese Wertung vom Teilnehmer online ist
        /// </summary>
        public bool IsOnline
        {
            get => _isOnline; set
            {
                if (_isOnline == value)
                    return;

                _isOnline = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Jeder Versuch in jeder Disziplin wird auf -1 gesetzt
        /// </summary>
        internal void Reset()
        {
            foreach (var disziplin in Disziplinen)
            {
                disziplin.Reset();
            }
        }

        /// <summary>
        /// Anzahl der Versuche die eingegeben wurden ( 0 - 24 )
        /// </summary>
        /// <returns></returns>
        internal int VersucheCount()
        {
            return Disziplinen.Sum(d => d.VersucheCount());
        }

        /// <summary>
        /// True, wenn alle Versuche eingegeben wurdne (24 Veruche)
        /// </summary>
        /// <returns></returns>
        internal bool VersucheAllEntered()
        {
            return VersucheCount() == 24;
        }


        #region READONLY  Punkte

        public int GesamtPunkte
        {
            get
            {
                return Disziplinen.Sum(d => d.Summe);
            }
        }

        public int PunkteMassenMitte
        {
            get
            {
                return Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Summe;
            }
        }

        public int PunkteSchuesse
        {
            get
            {
                return Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Summe;
            }
        }

        public int PunkteMassenSeitlich
        {
            get
            {
                return Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Summe;
            }
        }

        public int PunkteKombinieren
        {
            get
            {
                return Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Summe;
            }
        }

        #endregion



    }
}

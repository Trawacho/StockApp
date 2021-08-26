using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace StockApp.BaseClasses.Zielschiessen
{
    public class Teilnehmer : TPlayer
    {
        #region Fields

        private readonly ObservableCollection<Wertung> _wertungen = new ObservableCollection<Wertung>();
        private string _vereinsname;
        private int _aktuelleBahn;

        #endregion


        /// <summary>
        /// PropertyChanged für <see cref="Name"/> wird ausgelöst, wenn sich bei einer Wertung <see cref="Wertung.IsOnline"/> ändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WertungChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Wertung.IsOnline))
            {
                RaisePropertyChanged(nameof(Name));
            }
        }


        public Teilnehmer()
        {
            Startnummer = -1;
            AddNewWertung();
        }

        /// <summary>
        /// AnzeigeName. Wenn Online, mit Bahnnummer
        /// </summary>
        public new string Name
        {
            get
            {
                return Onlinewertung != null
                                ? $"{base.Name} ({AktuelleBahn}) "
                                : base.Name;
            }
        }

        public int Startnummer { get; set; }

        public string Vereinsname
        {
            get => _vereinsname;
            set
            {
                if (_vereinsname == value)
                    return;

                _vereinsname = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Wert < 0 wenn Teilnehmer aktuell auf einer Bahn spielt.
        /// </summary>
        public int AktuelleBahn
        {
            get => _aktuelleBahn;
            private set
            {
                if (_aktuelleBahn == value)
                    return;

                _aktuelleBahn = value;
                RaisePropertyChanged();

            }
        }

        /// <summary>
        /// Die Wertung aus <see cref="Wertungen"/>, die <see cref="Wertung.IsOnline"/> == true ist
        /// </summary>
        public Wertung Onlinewertung
        {
            get
            {
                return Wertungen.FirstOrDefault(w => w.IsOnline) ?? null;
            }
        }

        /// <summary>
        /// True, wenn <see cref="Onlinewertung"/> != null ist
        /// </summary>
        public bool HasOnlineWertung
        {
            get
            {
                return Onlinewertung != null;
            }
        }

        /// <summary>
        /// Setzt den Wert für <see cref="AktuelleBahn"/>. 
        /// <br>Die <see cref="Wertung"/> wird <see cref="Wertung.IsOnline"/> = true gesetzt.</br>
        /// <br>Die restlichen auf FALSE</br>
        /// </summary>
        /// <param name="bahnNummer"></param>
        public void SetAktuelleBahn(int bahnNummer, int wertungsNummer)
        {
            AktuelleBahn = bahnNummer;
            _wertungen.ToList().ForEach(w => w.IsOnline = false);
            _wertungen.First(w => w.Nummer == wertungsNummer).IsOnline = true;
            RaisePropertyChanged(nameof(this.HasOnlineWertung));
            RaisePropertyChanged(nameof(this.Onlinewertung));
        }

        /// <summary>
        /// <see cref="AktuelleBahn"/> wird auf -1 gesetzt. Alle <see cref="Wertungen"/> werden <see cref="Wertung.IsOnline"/> auf FALSE gesetzt
        /// </summary>
        public void DeleteAktuellBahn()
        {
            AktuelleBahn = -1;
            _wertungen.ToList().ForEach(w => w.IsOnline = false);
            RaisePropertyChanged(nameof(this.HasOnlineWertung));
            RaisePropertyChanged(nameof(this.Onlinewertung));
        }

        /// <summary>
        /// Wenn möglich, wird die nächste Wertung online geschaltet, oder Spieler geht offline
        /// </summary>
        public void SetWertungOfflineOrNext()
        {
            int aktuelleWertung = Onlinewertung.Nummer;

            _wertungen.ToList().ForEach(w => w.IsOnline = false);
            if (_wertungen.Any(w => w.Nummer == aktuelleWertung + 1))
            {
                var w = _wertungen.First(w => w.Nummer == aktuelleWertung + 1);
                if (!w.VersucheAllEntered())
                    _wertungen.First(w => w.Nummer == aktuelleWertung + 1).IsOnline = true;
            }

            if (!_wertungen.Any(w => w.IsOnline))
                AktuelleBahn = -1;

            RaisePropertyChanged(nameof(this.HasOnlineWertung));
            RaisePropertyChanged(nameof(this.Onlinewertung));
        }

        /// <summary>
        /// Liste der Wertungen
        /// </summary>
        public ObservableCollection<Wertung> Wertungen
        {
            get
            {
                return _wertungen;
            }
        }

        /// <summary>
        /// Es wird der Versuch in der OnlineWertung eingetragen
        /// </summary>
        /// <param name="versuchNr"></param>
        /// <param name="value"></param>
        internal void SetVersuch(int versuchNr, int value)
        {
            if (Onlinewertung == null)
                return;

            switch (versuchNr)
            {
                case 1:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch1 = value; break;
                case 2:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch2 = value; break;
                case 3:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch3 = value; break;
                case 4:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch4 = value; break;
                case 5:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch5 = value; break;
                case 6:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Versuch6 = value; break;

                case 7:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch1 = value; break;
                case 8:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch2 = value; break;
                case 9:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch3 = value; break;
                case 10:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch4 = value; break;
                case 11:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch5 = value; break;
                case 12:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Versuch6 = value; break;

                case 13:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch1 = value; break;
                case 14:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch2 = value; break;
                case 15:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch3 = value; break;
                case 16:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch4 = value; break;
                case 17:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch5 = value; break;
                case 18:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.MasseSeite).Versuch6 = value; break;

                case 19:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch1 = value; break;
                case 20:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch2 = value; break;
                case 21:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch3 = value; break;
                case 22:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch4 = value; break;
                case 23:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch5 = value; break;
                case 24:
                    this.Onlinewertung.Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Versuch6 = value; break;

                default:
                    break;
            }
        }

        /// <summary>
        /// True, wenn eine neue Wertung angefügt werden kann
        /// </summary>
        internal bool CanAddWertung()
        {
            return Wertungen.Count() < 5;
        }

        /// <summary>
        /// True, wenn eine Wertung entfernt werden kann
        /// </summary>
        internal bool CanRemoveWertung()
        {
            return Wertungen.Count() > 1;
        }

        /// <summary>
        /// Eine zusätzliche Wertung am Ende der Liste anfügen
        /// </summary>
        /// <param name="wertung"></param>
        internal void AddNewWertung()
        {
            var wertung = new Wertung
            {
                Nummer = this._wertungen.Count + 1
            };
            wertung.PropertyChanged += WertungChanged;
            this._wertungen.Add(wertung);
        }

        /// <summary>
        /// Die Wertung aus der Liste entfernen und neu nummerieren
        /// </summary>
        /// <param name="wertung"></param>
        internal void RemoveWertung(Wertung wertung)
        {
            this._wertungen.Remove(wertung);
            for (int i = 0; i < _wertungen.Count; i++)
            {
                _wertungen[i].Nummer = i + 1;
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StockApp.Interfaces;

namespace StockApp.BaseClasses.Zielschiessen
{
    public class Zielbewerb : TBaseBewerb, IZielbewerb
    {
        /// <summary>
        /// Liste der Teilnehmer
        /// </summary>
        private readonly List<Teilnehmer> _teilnehmerliste;

        /// <summary>
        /// fixtive Liste mit Nummern für Bahnen, die zur Auswahl stehen
        /// </summary>
        private readonly List<int> _zielBahnen;

        public Zielbewerb()
        {
            _teilnehmerliste = new List<Teilnehmer>();
            AddNewTeilnehmer();

            _zielBahnen = new List<int>();

            for (int i = 1; i <= 15; i++)
            {
                _zielBahnen.Add(i);
            }
        }

        /// <summary>
        /// Liste aller Teilnehmer
        /// </summary>
        public IOrderedEnumerable<Teilnehmer> Teilnehmerliste => _teilnehmerliste.AsReadOnly().OrderBy(t => t.Startnummer);

        /// <summary>
        /// Auflistung aller Bahnen (15 Bahnen werden automatisch ergzeugt)
        /// </summary>
        public IOrderedEnumerable<int> Bahnen => _zielBahnen.OrderBy(b => b);


        /// <summary>
        /// Auflistung aller freier Bahnen
        /// </summary>
        public IOrderedEnumerable<int> FreieBahnen
        {
            get
            {
                var belegteBahnen = Teilnehmerliste.Where(t => t.AktuelleBahn > 0).Select(b => b.AktuelleBahn);
                return Bahnen.Where(b => !belegteBahnen.Contains(b)).OrderBy(x => x);
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn sich der Status <see cref="Teilnehmer.HasOnlineWertung"/> bei einem <see cref="Teilnehmer"/> ändert.
        /// <br>Startet den <see cref="NetworkService"/> wenn der erste <see cref="Teilnehmer"/> online geht</br>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TeilnehmerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Teilnehmer.Wertungen)) RaisePropertyChanged(nameof(Teilnehmer.Wertungen));

            if (e.PropertyName != nameof(Teilnehmer.HasOnlineWertung)) return;

            if (Teilnehmerliste.Any(t => t.HasOnlineWertung) && !NetworkService.Instance.IsRunning())
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("NetworkService starten, da mindestens eine Wertung Online ist");
#endif
                NetworkService.Instance.Start();
            }

            RaisePropertyChanged(nameof(FreieBahnen));
        }

        /// <summary>
        /// Ein neuer Teilnehmer wird der Liste hinten hinzugefügt. 
        /// </summary>
        public void AddNewTeilnehmer()
        {
            AddTeilnehmer(new Teilnehmer());
        }

        /// <summary>
        /// Einen Teilnehmer anfügen
        /// </summary>
        /// <param name="teilnehmer"></param>
        public void AddTeilnehmer(Teilnehmer teilnehmer)
        {
            if (teilnehmer.Startnummer < 1 ||
                _teilnehmerliste.Any(t => t.Startnummer == teilnehmer.Startnummer))
                teilnehmer.Startnummer = _teilnehmerliste.Count + 1;
                

            teilnehmer.PropertyChanged += TeilnehmerPropertyChanged;
            _teilnehmerliste.Add(teilnehmer);
            RaisePropertyChanged(nameof(Teilnehmerliste));
        }

        /// <summary>
        /// True, wenn die Teilnehmerliste nicht voll ist (<= 30)
        /// </summary>
        /// <returns></returns>
        public bool CanAddTeilnehmer()
        {
            return _teilnehmerliste.Count() <= 30;
        }

        /// <summary>
        /// Der Teilnehmer wird aus der Liste entfernt
        /// </summary>
        /// <param name="teilnehmer"></param>
        public void RemoveTeilnehmer(Teilnehmer teilnehmer)
        {
            _teilnehmerliste.Remove(teilnehmer);
            for (int i = 0; i < _teilnehmerliste.Count; i++)
            {
                _teilnehmerliste[i].Startnummer = i + 1;
            }
            RaisePropertyChanged(nameof(Teilnehmerliste));
        }

        internal IEnumerable<Teilnehmer> GetTeilnehmerRanked() => _teilnehmerliste.OrderByDescending(t => t.Wertungen.Sum(w => w.GesamtPunkte));

        /// <summary>
        /// True, solange die Anzahl der Teilnehmeer > 1 ist
        /// </summary>
        /// <returns></returns>
        public bool CanRemoveTeilnehmer() => _teilnehmerliste.Count() > 1;

        public void MoveTeilnehmer(int oldIndex, int newIndex)
        {
            var teilnehmer = _teilnehmerliste[oldIndex];

            if (newIndex < 0)
            {
                throw new ArgumentOutOfRangeException("neue Startnummer darf nicht kleiner 1 sein");
            }

            _teilnehmerliste.RemoveAt(oldIndex);
            _teilnehmerliste.Insert(newIndex, teilnehmer);

            for (int i = 0; i < _teilnehmerliste.Count; i++)
            {
                _teilnehmerliste[i].Startnummer = i + 1;
            }

            RaisePropertyChanged(nameof(this.Teilnehmerliste));
        }

        private NetworkTelegram lastTelegram;
        public override void SetBroadcastData(NetworkTelegram telegram)
        {
            if (telegram.StockTVModus != 100) return;
            if (telegram.Equals(lastTelegram)) return;
            lastTelegram = telegram.Copy();

            /*
             * 03 01 00 00 00 00 00 00 00 00 04 08 00 06 10 02 05 10 02 00 10 
             * 
             * Aufbau: 
             * Im den ersten zehn Bytes (Header) stehen Settings von StocktV wie Bahnnummer (Position1) usw
             * In jedem weiteren Byte kommen die laufenden Versuche (max 24) 
             * Somit ist ein Datagramm max 10+24 Bytes lang
             * 
             */

            try
            {
         
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Bahnnummer:{telegram.BahnNummer} -- {string.Join("-", telegram.Values)}");
#endif

                if (this.Teilnehmerliste.FirstOrDefault(t => t.AktuelleBahn == telegram.BahnNummer) is Teilnehmer spieler)
                {
                    if (spieler.Onlinewertung.VersucheAllEntered() && telegram.Values.Length == 0)  //Alle Versuche auf der entsprechenden Bahn wurden eingegeben und von StockTV kommen keine Values, nur der Header
                    {
                        spieler.SetWertungOfflineOrNext();
                    }
                    else
                    {
                        spieler.Onlinewertung.Reset();

                        for (int i = 0; i < telegram.Values.Length; i++)
                        {
                            spieler?.SetVersuch(i + 1, telegram.Values[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetBroadcastData: {ex.Message}");
            }
            finally
            {
                RaisePropertyChanged("");
            }

        }

    }
}

using StockApp.BaseClasses;
using StockApp.BaseClasses.Zielschiessen;
using StockApp.Interfaces;
using System;
using System.Collections.Generic;

namespace StockApp.ViewModels
{
    public class TurnierViewModel : BaseViewModel, ITurnierViewModel
    {

        private readonly Turnier Turnier;

        public TurnierViewModel(Turnier turnier)
        {
            this.Turnier = turnier;
            Wettbewerbsarten = EnumUtil.GetEnumList<Wettbewerbsart>();
        }


        /// <summary>
        /// Veranstaltungsort
        /// </summary>
        public string TurnierOrt
        {
            get => Turnier.OrgaDaten.Venue;
            set
            {
                if (Turnier.OrgaDaten.Venue == value)
                    return;

                Turnier.OrgaDaten.Venue = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Veranstalter
        /// </summary>
        public string Veranstalter
        {
            get
            {
                return Turnier.OrgaDaten.Organizer;
            }
            set
            {
                if (Turnier.OrgaDaten.Organizer == value)
                    return;

                Turnier.OrgaDaten.Organizer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Durchführer
        /// </summary>
        public string Durchfuehrer
        {
            get
            {
                return Turnier.OrgaDaten.Operator;
            }
            set
            {
                if (Turnier.OrgaDaten.Operator == value)
                    return;

                Turnier.OrgaDaten.Operator = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Name des Turniers
        /// </summary>
        public string TurnierName
        {
            get
            {
                return Turnier.OrgaDaten.TournamentName;
            }
            set
            {
                if (Turnier.OrgaDaten.TournamentName == value)
                    return;

                Turnier.OrgaDaten.TournamentName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Zeitpunkt des Turniers
        /// </summary>
        public DateTime TurnierDatum
        {
            get
            {
                return Turnier.OrgaDaten.DateOfTournament;
            }
            set
            {
                if (Turnier.OrgaDaten.DateOfTournament == value)
                    return;

                Turnier.OrgaDaten.DateOfTournament = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Startgebühr
        /// </summary>
        public Startgebuehr Startgebuehr
        {
            get
            {
                return Turnier.OrgaDaten.EntryFee;
            }
            set
            {
                Turnier.OrgaDaten.EntryFee = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Schiedsrichter
        /// </summary>
        public Schiedsrichter Schiedsrichter
        {
            get
            {
                return Turnier.OrgaDaten.Referee;
            }
            set
            {
                Turnier.OrgaDaten.Referee = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Wettbewerbsleiter
        /// </summary>
        public Wettbewerbsleiter Wettbewerbsleiter
        {
            get
            {
                return Turnier.OrgaDaten.CompetitionManager;
            }
            set
            {
                Turnier.OrgaDaten.CompetitionManager = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Rechenbüro
        /// </summary>
        public Rechenbuero Rechenbuero
        {
            get
            {
                return Turnier.OrgaDaten.ComputingOfficer;
            }
            set
            {
                Turnier.OrgaDaten.ComputingOfficer = value;
                RaisePropertyChanged();
            }
        }

        public List<Wettbewerbsart> Wettbewerbsarten { get; set; }

        public Wettbewerbsart Wettbewerbsart
        {
            get
            {
                return Turnier.Wettbewerb.GetType() == typeof(TeamBewerb)
                            ? Wettbewerbsart.Team
                            : Wettbewerbsart.Ziel;
            }
            set
            {
                if (value == Wettbewerbsart.Ziel)
                {
                    Turnier.SetBewerb(Wettbewerbsart.Ziel);
                }
                else
                {
                    Turnier.SetBewerb(Wettbewerbsart.Team);
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsSpielgruppeVisible));
            }
        }

        /// <summary>
        /// Spielgruppe
        /// Wenn mehrere Gruppen auf der Spielfäche unterschiedliche Turniere spielen
        /// </summary>
        public int SpielGruppe
        {
            get
            {
                return Turnier.Wettbewerb is TeamBewerb t 
                    ? t.SpielGruppe 
                    : 0;
            }
            set
            {
                if (Turnier.Wettbewerb is TeamBewerb t)
                {
                    t.SpielGruppe = value;
                }
                RaisePropertyChanged();
            }
        }

        public bool IsSpielgruppeVisible => Wettbewerbsart == Wettbewerbsart.Team;
    }

    public class TurnierDesignViewModel : ITurnierViewModel
    {
        public TurnierDesignViewModel()
        {
            Startgebuehr = new Startgebuehr(30.00, "dreißig");
            Wettbewerbsarten = EnumUtil.GetEnumList<Wettbewerbsart>();
        }

        public string TurnierOrt { get; set; } = "Hankofen";
        public string Veranstalter { get; set; } = "Kreis 105 Gäuboden-Vorwald";
        public string Durchfuehrer { get; set; } = "ESF Hankofen";

        public string TurnierName { get; set; } = "Kreispokal Herren auf Asphalt im Sommer";

        public DateTime TurnierDatum { get; set; } = DateTime.Now;

        public int SpielGruppe { get; set; } = 3;

        public Startgebuehr Startgebuehr { get; set; }

        public Schiedsrichter Schiedsrichter { get; set; }
        public Rechenbuero Rechenbuero { get; set; }
        public Wettbewerbsleiter Wettbewerbsleiter { get; set; }

        public List<Wettbewerbsart> Wettbewerbsarten { get; set; }

        public Wettbewerbsart Wettbewerbsart { get; set; }

        public bool IsSpielgruppeVisible { get; } = true;
    }


}


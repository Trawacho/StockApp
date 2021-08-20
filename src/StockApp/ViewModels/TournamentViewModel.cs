using StockApp.BaseClasses;
using StockApp.Interfaces;
using System;

namespace StockApp.ViewModels
{
    public class TournamentViewModel : BaseViewModel, ITournamentViewModel
    {

        private readonly Tournament Tournament;

        public TournamentViewModel(Tournament tournament)
        {
            this.Tournament = tournament;
        }


        /// <summary>
        /// Veranstaltungsort
        /// </summary>
        public string Venue
        {
            get { return Tournament.Venue; }
            set
            {
                if (Tournament.Venue == value)
                    return;

                Tournament.Venue = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Veranstalter
        /// </summary>
        public string Organizer
        {
            get
            {
                return Tournament.Organizer;
            }
            set
            {
                if (Tournament.Organizer == value)
                    return;

                Tournament.Organizer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Durchführer
        /// </summary>
        public string Operator
        {
            get
            {
                return Tournament.Operator;
            }
            set
            {
                if (Tournament.Operator == value)
                    return;
                Tournament.Operator = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Name des Turniers
        /// </summary>
        public string TournamentName
        {
            get
            {
                return Tournament.TournamentName;
            }
            set
            {
                if (Tournament.TournamentName == value)
                    return;

                Tournament.TournamentName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Zeitpunkt des Turniers
        /// </summary>
        public DateTime DateOfTournament
        {
            get
            {
                return Tournament.DateOfTournament;
            }
            set
            {
                if (Tournament.DateOfTournament == value)
                    return;
                Tournament.DateOfTournament = value;
                RaisePropertyChanged();
            }
        }

        public EntryFee EntryFee
        {
            get
            {
                return Tournament.EntryFee;
            }
            set
            {
                Tournament.EntryFee = value;
                RaisePropertyChanged();
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
                return Tournament.SpielGruppe;
            }
            set
            {
                Tournament.SpielGruppe = value;
                RaisePropertyChanged();
            }
        }

        public Referee Referee
        {
            get
            {
                return Tournament.Referee;
            }
            set
            {
                Tournament.Referee = value;
                RaisePropertyChanged();
            }
        }

        public CompetitionManager CompetitionManager
        {
            get
            {
                return Tournament.CompetitionManager;
            }
            set
            {
                Tournament.CompetitionManager = value;
                RaisePropertyChanged();
            }
        }

        public ComputingOfficer ComputingOfficer
        {
            get
            {
                return Tournament.ComputingOfficer;
            }
            set
            {
                Tournament.ComputingOfficer = value;
                RaisePropertyChanged();
            }
        }
    }

    public class TournamentDesignViewModel : ITournamentViewModel
    {
        public TournamentDesignViewModel()
        {
            EntryFee = new EntryFee(30.00, "dreißig");
        }

        public string Venue { get; set; } = "Hankofen";
        public string Organizer { get; set; } = "Kreis 105 Gäuboden-Vorwald";
        public string Operator { get; set; } = "ESF Hankofen";

        public string TournamentName { get; set; } = "Kreispokal Herren auf Asphalt im Sommer";

        public DateTime DateOfTournament { get; set; } = DateTime.Now;

        public int SpielGruppe { get; set; } = 3;
        public EntryFee EntryFee { get; set; }

        public Referee Referee { get; set; }
        public ComputingOfficer ComputingOfficer { get; set; }
        public CompetitionManager CompetitionManager { get; set; }
    }


}

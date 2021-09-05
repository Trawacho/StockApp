using StockApp.BaseClasses;
using StockApp.Commands;
using StockApp.Interfaces;
using StockApp.Output;
using StockApp.Output.WertungsKarteBase;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class GamesViewModel : BaseViewModel, IGamesViewModel
    {
        #region Fields

        private readonly TeamBewerb tournament;
        private readonly StockTVs stockTVs;

        #endregion

        #region Constructor

        public GamesViewModel(TeamBewerb tournament, StockTVs stockTVs)
        {
            this.tournament = tournament;
            this.stockTVs = stockTVs;
            ConcatRoundsOnOutput = false;
            TeamNameOnTurnCards = false;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Anzahl der Spielbahnen
        /// </summary>
        public int NumberOfCourts => tournament.NumberOfCourts;


        /// <summary>
        /// Anzahl der Spielrunden
        /// </summary>
        public int NumberOfGameRounds
        {
            get => tournament.NumberOfGameRounds;
            set
            {
                if (tournament.NumberOfGameRounds == value) return;

                tournament.NumberOfGameRounds = value;
                tournament.RemoveAllGames();
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// bei gerader Anzahl an Manschaften, true, dann werden zwei Aussetzer gespielt
        /// </summary>
        public bool TwoPauseGames
        {
            get => tournament.TwoPauseGames;
            set
            {
                if (tournament.TwoPauseGames == value) return;

                tournament.TwoPauseGames = value;
                tournament.RemoveAllGames();
                RaisePropertyChanged();
            }
        }


        public bool IsStartOfGameChanged
        {
            get => tournament.NumberOfGameRounds > 1 && tournament.StartingTeamChange;
            set
            {
                if (tournament.StartingTeamChange == value) return;

                tournament.StartingTeamChange = value;
                tournament.RemoveAllGames();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Liste der Reellen Teams
        /// </summary>
        public ObservableCollection<Team> Teams => new(tournament.Teams.Where(t => !t.IsVirtual));


        public bool IsDirectionOfCourtsFromRightToLeft
        {
            get => tournament.IsDirectionOfCourtsFromRightToLeft;
            set
            {
                if (tournament.IsDirectionOfCourtsFromRightToLeft == value) return;

                tournament.IsDirectionOfCourtsFromRightToLeft = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DirectionOfCourtsDescription));
            }
        }

        public string DirectionOfCourtsDescription => IsDirectionOfCourtsFromRightToLeft
                                                        ? "1. Bahn rechts, weitere folgen links"
                                                        : "1. Bahn links, weitere folgen rechts";


        private bool concatRoundsOnOutput;
        public bool ConcatRoundsOnOutput
        {
            get => NumberOfGameRounds > 1 && concatRoundsOnOutput;
            set => SetProperty(ref concatRoundsOnOutput, value);
        }

        public bool TeamNameOnTurnCards { get; set; }

        public bool Is8KehrenSpiel
        {
            get => tournament.Is8TurnsGame;
            set 
            {
                if (tournament.Is8TurnsGame == value) return;
                tournament.Is8TurnsGame = value;
                RaisePropertyChanged();
            }
        }

        private bool isTurnCardForStockTv;
        public bool IsTurnCardForStockTV
        {
            get => isTurnCardForStockTv;
            set => SetProperty(ref isTurnCardForStockTv, value);
        }

        #endregion

        #region Commands

        private ICommand _createGamesCommand;
        public ICommand CreateGamesCommand
        {
            get
            {
                return _createGamesCommand ??= new RelayCommand(
                    (p) =>
                    {
                        tournament.ReCreateGames();
                        RaisePropertyChanged(nameof(Teams));
                    }
                    );

            }
        }
        
        
        private ICommand _sendTeamsToStockTVCommand;
        public ICommand SendTeamsToStockTVCommand
        {
            get
            {
                return _sendTeamsToStockTVCommand ??= new RelayCommand(
                    (p) =>
                    {
                        var bahnNummern = tournament.GetAllGames().Where(b => b.CourtNumber > 0) .Select(x => x.CourtNumber).Distinct();
                        var spielGruppe = tournament.SpielGruppe;
                        foreach (var bahn in bahnNummern)
                        {
                            var stockTV = stockTVs?.FirstOrDefault(a => a.TVSettings.Spielgruppe == spielGruppe && a.TVSettings.Bahn == bahn);

                            if (stockTV == null) continue;

                            var spieleAufBahnX = tournament.GetAllGames()
                                                            .Where(g => g.CourtNumber == bahn)
                                                            .Select(b => new StockTVBegegnung()
                                                            {
                                                                SpielNummer = b.GameNumber,
                                                                TeamNameA = b.TeamA.TeamName,
                                                                TeamNameB = b.TeamB.TeamName
                                                            });

                            stockTV.SendTeamNames(spieleAufBahnX);
                        }
                    },
                    (p) => tournament.GetAllGames().Count() > 1
                    );
            }
        }


        private ICommand _printTurnCardsCommand;
        public ICommand PrintTurnCardsCommand
        {
            get
            {
                return _printTurnCardsCommand ??= new RelayCommand(
                    (p) =>
                    {
                        var x = IsTurnCardForStockTV
                                                     ? new Output.WertungskarteStockTV.WertungskarteStockTV()
                                                     : new Output.Wertungskarte.Wertungskarte() as WertungskarteBase;

                        PrintPreview printPreview = new PrintPreview();
                        var A4Size = new System.Windows.Size(8 * 96, 11.5 * 96);
                        printPreview.Document = x.GetDocument(A4Size, tournament, TeamNameOnTurnCards, ConcatRoundsOnOutput);
                        printPreview.ShowDialog();
                    },
                    (p) =>
                    {
                        return tournament.GetAllGames().Count() > 1;
                    }
                    );
            }
        }
        

        private ICommand _printBahnblockCommand;
        public ICommand PrintBahnblockCommand
        {
            get
            {
                return _printBahnblockCommand ??= new RelayCommand(
                    (p) =>
                    {
                        var x = new Output.Bahnblock.BahnBlock();
                        PrintPreview printPreview = new PrintPreview();
                        var A4Size = new System.Windows.Size(8 * 96, 11.5 * 96);
                        printPreview.Document = x.GetDocument(A4Size, tournament);
                        printPreview.ShowDialog();
                    },
                    (p) =>
                    {
                        return tournament.GetAllGames().Count() > 1;
                    }
                    );
            }
        }

        #endregion
    }

    public class GamesDesignViewModel : IGamesViewModel
    {
        private readonly TeamBewerb t = new TeamBewerb();

        public GamesDesignViewModel()
        {
            t.CreateGames();
        }

        public int NumberOfCourts { get; }
        public int NumberOfGameRounds { get; set; } = 1;
        public bool TwoPauseGames { get; set; } = true;

        public ObservableCollection<Team> Teams { get { return new ObservableCollection<Team>(t.Teams.Where(t => !t.IsVirtual)); } }

        public bool ConcatRoundsOnOutput { get; set; } = true;
        public bool TeamNameOnTurnCards { get; set; } = true;

        public bool IsDirectionOfCourtsFromRightToLeft { get; set; } = true;
        public string DirectionOfCourtsDescription
        {
            get
            {
                return IsDirectionOfCourtsFromRightToLeft
                     ? "1. Bahn rechts, weitere folgen links"
                     : "1. Bahn links, weitere folgen rechts";
            }
        }

        public bool Is8KehrenSpiel { get; set; } = true;
        public bool IsTurnCardForStockTV { get; set; } = true;
        public bool IsStartOfGameChanged { get; set; } = true;

        public ICommand CreateGamesCommand { get; }
        public ICommand SendTeamsToStockTVCommand { get; }
        public ICommand PrintTurnCardsCommand { get; }
        public ICommand PrintBahnblockCommand { get; }
    }
}

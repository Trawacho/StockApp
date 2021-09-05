using StockApp.BaseClasses;
using StockApp.BaseClasses.Zielschiessen;
using StockApp.Commands;
using StockApp.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        #region Fields

        private readonly IDialogService dialogService;

        private BaseViewModel _viewModel;

        private Turnier _turnier;

        private string tournamentFileName = string.Empty;

        private StockTVs _stockTVs;

        #endregion

        #region Properties

        /// <summary>
        /// Holds the ViewModel for the UserControl in the mid of the page
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => _viewModel;
            set => SetProperty(ref _viewModel, value);

        }

        /// <summary>
        /// Action to Close the Application
        /// </summary>
        public Action ExitApplicationAction { get; set; }

        /// <summary>
        /// Content for the ListenerButton with state related content
        /// </summary>
        public string UdpButtonContent
        {
            get
            {
                return NetworkService.Instance.IsRunning()
                            ? "Stop Listener"
                            : "Start Listener";
            }
        }

        /// <summary>
        /// Zeigt die Versionsnummer vom Assembly
        /// </summary>
        public string VersionNumber
        {
            get
            {
                return $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
            }
        }

        public bool IsTeamBewerb
        {
            get
            {
                return _turnier.Wettbewerb is TeamBewerb;
            }
        }

        public bool IsZielBewerb
        {
            get
            {
                return _turnier.Wettbewerb is Zielbewerb;
            }
        }

        public string StockTVCount
        {
            get
            {
                return $"StockTV: {_stockTVs?.Count ?? 0}x";
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterless Constructror for DesignView and as BaseConstructor
        /// </summary>
        public MainViewModel()
        {
            SetNewTurnier(new Turnier());

            NetworkService.Instance.StartStopStateChanged += NetworkService_StartStopStateChanged;
            NetworkService.Instance.DataReceived += NetworkService_DataReceived;

            _stockTVs = new StockTVs();
            _stockTVs.StockTVCollectionAdded += StockTVs_StockTVCollectionAdded;
            _stockTVs.StockTVCollectionRemoved += StockTVs_StockTVCollectionRemoved;
            _stockTVs.StartDiscovery();
        }

        private void SetNewTurnier(Turnier t)
        {
            if (_turnier != null)
            {
                _turnier.PropertyChanged -= Turnier_PropertyChanged;
                _turnier.SpielgruppeChanged -= Turnier_SpielgruppeChanged;
            }

            _turnier = t;
            t.SetBewerb(Wettbewerbsart.Team);
            
            RaisePropertyChanged(nameof(IsZielBewerb));
            RaisePropertyChanged(nameof(IsTeamBewerb));
            ViewModel = new TurnierViewModel(_turnier);
            RaisePropertyChanged(nameof(WindowTitle));
            _turnier.PropertyChanged += Turnier_PropertyChanged;
            _turnier.SpielgruppeChanged += Turnier_SpielgruppeChanged;
        }

        private void Turnier_SpielgruppeChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }

        private void NetworkService_DataReceived(object sender, NetworkServiceDataReceivedEventArgs e)
        {
            this._turnier.Wettbewerb.SetBroadcastData(e.NetworkTelegram);
        }

        private void NetworkService_StartStopStateChanged(object sender, NetworkServiceStateEventArgs e)
        {
            RaisePropertyChanged(nameof(UdpButtonContent));
        }



        /// <summary>
        /// Default-Constructor
        /// </summary>
        /// <param name="dialogService"></param>
        public MainViewModel(IDialogService dialogService) : this()
        {
            this.dialogService = dialogService;
        }

        #endregion

        private void StockTVs_StockTVCollectionRemoved(object sender, StockTVCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(StockTVCount));
        }

        private void StockTVs_StockTVCollectionAdded(object sender, StockTVCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(StockTVCount));
        }


        private void Turnier_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Turnier.Wettbewerb))
            {
                RaisePropertyChanged(nameof(this.IsTeamBewerb));
                RaisePropertyChanged(nameof(this.IsZielBewerb));
            }
        }



        public string WindowTitle
        {
            get
            {
                if ((_turnier.Wettbewerb is TeamBewerb t && t.SpielGruppe == 0) ||
                    _turnier.Wettbewerb is Zielbewerb)
                    return "StockApp";
                else
                {
                    return $"StockApp --> Gruppe:{(_turnier.Wettbewerb as TeamBewerb).SpielGruppeString()}";
                }
            }
        }

        #region Commands

        private ICommand _showLiveResultCommand;
        public ICommand ShowLiveResultCommand
        {
            get
            {
                return _showLiveResultCommand ??= new RelayCommand(
                    (p) =>
                    {
                        dialogService.SetOwner(App.Current.MainWindow);
                        if (this.IsTeamBewerb)
                            dialogService.Show(
                                  new LiveResultViewModel(_turnier)
                                  );
                        else if (this.IsZielBewerb)
                            dialogService.Show(
                                new LiveZielResultViewModel(_turnier)
                                );
                    },
                    (p) =>
                    {
                        return true;
                    }
                    );
            }
        }

        private ICommand _startStopUdpReceiverCommand;
        public ICommand StartStopUdpReceiverCommand
        {
            get
            {
                return _startStopUdpReceiverCommand ??=
                    new RelayCommand(
                            (p) =>
                            {

                                if (NetworkService.Instance.IsRunning())
                                    NetworkService.Instance.Stop();
                                else
                                    NetworkService.Instance.Start();

                                RaisePropertyChanged(nameof(UdpButtonContent));
                            },
                            (o) => { return true; }
                            );
            }
        }

        private ICommand _showTournamentViewCommand;
        public ICommand ShowTournamentViewCommand
        {
            get
            {
                return _showTournamentViewCommand ??= new RelayCommand(
                    (p) =>
                    {
                        ViewModel = new TurnierViewModel(_turnier);
                    },
                    (p) => true
                    );
            }
        }

        private ICommand _showTeamsViewCommand;
        public ICommand ShowTeamsViewCommand
        {
            get
            {
                return _showTeamsViewCommand ??= new RelayCommand(
                    (p) =>
                    {
                        this.ViewModel = new TeamsViewModel(_turnier);
                    },
                    (p) =>
                    {
                        return IsTeamBewerb;
                    }
                    ); ;
            }
        }

        private ICommand _showGamesViewCommand;
        public ICommand ShowGamesViewCommand
        {
            get
            {
                return _showGamesViewCommand ??= new RelayCommand(
                    (p) =>
                    {
                        this.ViewModel = new GamesViewModel(_turnier.Wettbewerb as TeamBewerb, _stockTVs);
                    },
                    (p) =>
                    {
                        return (_turnier.Wettbewerb as TeamBewerb)?.Teams.Count > 0;
                    });
            }
        }

        private ICommand _showResultsViewCommand;
        public ICommand ShowResultsViewCommand
        {
            get
            {
                return _showResultsViewCommand ??= new RelayCommand(
                    (p) =>
                    {
                        this.ViewModel = new ResultsViewModel(_turnier);
                    },
                    (p) =>
                    {
                        return (_turnier.Wettbewerb as TeamBewerb)?.CountOfGames() > 0;
                    });
            }
        }

        private ICommand _exitApplicationCommand;
        public ICommand ExitApplicationCommand
        {
            get
            {
                return _exitApplicationCommand ??= new RelayCommand(
                    (p) =>
                    {
                        ExitApplicationAction();
                    });
            }
        }

        private ICommand _newTournamentCommand;
        public ICommand NewTournamentCommand
        {
            get
            {
                return _newTournamentCommand ??= new RelayCommand(
                    (p) =>
                    {
                        SetNewTurnier(new Turnier());
                    });
            }
        }

        private ICommand _saveTournamentCommand;
        public ICommand SaveTournamentCommand
        {
            get
            {
                return _saveTournamentCommand ??= new RelayCommand(
                    (p) =>
                    {
                        Save(tournamentFileName);
                    });
            }
        }

        private ICommand _saveAsTournamentCommand;
        public ICommand SaveAsTournamentCommand
        {
            get
            {
                return _saveAsTournamentCommand ??= new RelayCommand(
                    (p) =>
                    {
                        Save(null);
                    });
            }
        }

        private ICommand _openTournamentCommand;
        public ICommand OpenTournamentCommand
        {
            get
            {
                return _openTournamentCommand ??= new RelayCommand(
                    (p) =>
                    {
                        try
                        {
                            var ofd = new OpenFileDialog
                            {
                                Filter = "StockMaster Files (*.skmr)|*.skmr",
                                DefaultExt = "skmr"
                            };

                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                var filePath = ofd.FileName;
                                SetNewTurnier(Turnier.Load(filePath));
                                this.tournamentFileName = filePath;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Fehler beim Öffnen:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    });
            }
        }

        private ICommand _showZielSpielerViewCommand;
        public ICommand ShowZielSpielerViewCommand
        {
            get
            {
                return
                   _showZielSpielerViewCommand ??= new RelayCommand(
                       (p) =>
                       {
                           this.ViewModel = new ZielSpielerViewModel(_turnier);
                       },
                       (p) =>
                       {
                           return (_turnier.Wettbewerb is Zielbewerb);
                       });
            }
        }


        private ICommand _showStockTVCollectionCommand;
        public ICommand ShowStockTVCollectionCommand
        {
            get
            {
                return
                    _showStockTVCollectionCommand ??= new RelayCommand(
                        (p) =>
                        {
                            this.ViewModel = new StockTVCollectionViewModel(ref _stockTVs);
                        },
                        (p) =>
                        {
                            return true;
                        });
            }
        }
        #endregion

        private void Save(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                var saveFileDlg = new SaveFileDialog
                {
                    DefaultExt = "skmr",
                    Filter = "StockApp File (*skmr)|*.skmr"
                };
                var dlgResult = saveFileDlg.ShowDialog();
                if (dlgResult == DialogResult.OK)
                {
                    fileName = saveFileDlg.FileName;
                }
            }

            if (String.IsNullOrEmpty(fileName)) return;

            try
            {
                Turnier.Save(_turnier, fileName);
                this.tournamentFileName = fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speicher:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void StopNetMq()
        {
            _stockTVs.StopAllServices();
        }




    }
}


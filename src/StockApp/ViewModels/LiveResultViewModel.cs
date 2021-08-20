using StockApp.BaseClasses;
using StockApp.Commands;
using StockApp.Dialogs;
using StockApp.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class LiveResultViewModel : BaseViewModel, IDialogRequestClose, ILiveResultViewModel
    {
        public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;
        public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;

        readonly Tournament tournament;
        readonly NetworkService networkService;

        public LiveResultViewModel(Tournament tournament, NetworkService networkService)
        {
            this.tournament = tournament;
            this.networkService = networkService;
            tournament.PropertyChanged += Tournament_PropertyChanged;
            networkService.StartStopStateChanged += NetworkService_StartStopStateChanged;
        }

        private void NetworkService_StartStopStateChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(IsListenerOnline));
        }

        private void Tournament_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Ergebnisliste));
        }

        private bool isLive;
        /// <summary>
        /// Zeige Ergebnis aus StockTV nach jeder Kehre (true) oder nach jedem Spiel (false)
        /// </summary>
        public bool IsLive
        {
            get
            {
                return isLive;
            }
            set
            {
                if (isLive == value)
                {
                    return;
                }
                isLive = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Ergebnisliste));
            }
        }

        private bool showStockPunkte;
        /// <summary>
        /// Zeige im DataGrid die StockPunkte
        /// </summary>
        public bool ShowStockPunkte
        {
            get { return showStockPunkte; }
            set
            {
                if (showStockPunkte == value) return;

                showStockPunkte = value;
                RaisePropertyChanged();
            }
        }

        private bool showDifferenz;
        /// <summary>
        /// Zeige im DataGrid die StockPunkteDifferenz
        /// </summary>
        public bool ShowDifferenz
        {
            get { return showDifferenz; }
            set
            {
                if (ShowDifferenz == value) return;

                showDifferenz = value;
                RaisePropertyChanged();

            }
        }

        /// <summary>
        /// Zeigt ob der Listener Online ist bzw. ändert den Status
        /// </summary>
        public bool IsListenerOnline
        {
            get
            {
                return this.networkService?.IsRunning() ?? false;
            }
            set
            {
                this.networkService.SwitchStartStopState();
                RaisePropertyChanged();
            }
        }

        public string WindowTitle
        {
            get
            {
                if (this.tournament.SpielGruppe == 0)
                    return "StockApp Live-Ergebnis";
                else
                {
                    return $"StockApp Live-Ergebnis --> Gruppe:{tournament.SpielGruppeString()}";
                }
            }
        }


        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {

                return _closeCommand ??= new RelayCommand(
                    (p) =>
                    {
                        WindowCloseRequested?.Invoke(this, new WindowCloseRequestedEventArgs());
                        DialogCloseRequested?.Invoke(null, null);
                    },
                    (p) => true);
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??= new RelayCommand(
                    (p) =>
                    {
                        RaisePropertyChanged(nameof(Ergebnisliste));
                    });
            }
        }

        public ObservableCollection<(int Platzierung, Team Team, bool isLive)> Ergebnisliste
        {
            get
            {
                var liste = new ObservableCollection<(int _platzierung, Team _team, bool _isLive)>();
                int i = 1;
                foreach (var t in tournament.GetTeamsRanked(IsLive))
                {
                    liste.Add((i, t, this.IsLive));
                    i++;
                }
                return liste;
            }
        }
    }

    public class LiveResultDesignViewModel : ILiveResultViewModel
    {
        public LiveResultDesignViewModel()
        {
            this.Ergebnisliste = new ObservableCollection<(int, Team, bool)>
            {
                (1, new Team("ESF Hankofen"),true),
                (2, new Team("TV Geiselhöring"), true),
                (3, new Team("EC EBRA Aiterhofen"), true),
                (4, new Team("EC Pilsting"), true),
                (5, new Team("DJK Leiblfing"), true),
                (6, new Team("EC Welchenberg"), true),
                (7, new Team("SV Salching"), true),
                (8, new Team("EC Straßkirchen"), true),
                (9, new Team("DJK Aigen am Inn"), true)
            };
        }
        public ObservableCollection<(int Platzierung, Team Team, bool isLive)> Ergebnisliste
        {
            get;
        }
        public bool IsLive { get; set; }
        public bool ShowDifferenz { get; set; } = false;
        public bool IsListenerOnline { get; set; } = false;
        public bool ShowStockPunkte { get; set; } = false;

        public string WindowTitle { get; } = "StockApp Live-Result";
        public ICommand CloseCommand { get; }
        public ICommand RefreshCommand { get; }
    }


}

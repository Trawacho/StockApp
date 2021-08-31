using StockApp.BaseClasses;
using StockApp.BaseClasses.Zielschiessen;
using StockApp.Commands;
using StockApp.Dialogs;
using StockApp.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class LiveResultViewModel : BaseViewModel, IDialogRequestClose, ILiveResultViewModel, IDisposable
    {
        public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;
        public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;

        readonly Turnier turnier;
        readonly TeamBewerb bewerb;
        readonly NetworkService networkService;
       
        public LiveResultViewModel(Turnier turnier)
        {
            if (turnier.Wettbewerb is Zielbewerb) throw new NotImplementedException("Zielbewerb ist nicht implementiert");
           
            this.turnier = turnier;
            this.bewerb = turnier.Wettbewerb as TeamBewerb;
            this.networkService = NetworkService.Instance;
            this.bewerb.PropertyChanged += Tournament_PropertyChanged;
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

        public void Dispose()
        {
            this.networkService.StartStopStateChanged -= NetworkService_StartStopStateChanged;
        }

        private bool isLive;
        /// <summary>
        /// Zeige Ergebnis aus StockTV nach jeder Kehre (true) oder nach jedem Spiel (false)
        /// </summary>
        public bool IsLive
        {
            get => isLive;
            set
            {
                if (SetProperty(ref isLive, value))
                    RaisePropertyChanged(nameof(Ergebnisliste));
            }
        }

        private bool showStockPunkte;
        /// <summary>
        /// Zeige im DataGrid die StockPunkte
        /// </summary>
        public bool ShowStockPunkte
        {
            get => showStockPunkte;
            set => SetProperty(ref showStockPunkte, value);
        }

        private bool showDifferenz;
        /// <summary>
        /// Zeige im DataGrid die StockPunkteDifferenz
        /// </summary>
        public bool ShowDifferenz
        {
            get => showDifferenz;
            set => SetProperty(ref showDifferenz, value);
        }

        /// <summary>
        /// Zeigt ob der Listener Online ist bzw. ändert den Status
        /// </summary>
        public bool IsListenerOnline
        {
            get => this.networkService.IsRunning();
            set
            {
                if (this.networkService.IsRunning())
                    this.networkService.Stop();
                else
                    this.networkService.Start();

                RaisePropertyChanged();
            }
        }

        public string WindowTitle
        {
            get
            {
                var w = turnier.Wettbewerb as TeamBewerb;
                if (w.SpielGruppe == 0)
                    return "StockApp Live-Ergebnis";
                else
                {
                    return $"StockApp Live-Ergebnis --> Gruppe:{w.SpielGruppeString()}";
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
                foreach (var t in bewerb.GetTeamsRanked(IsLive))
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


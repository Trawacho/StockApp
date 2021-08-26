using StockApp.BaseClasses;
using StockApp.BaseClasses.Zielschiessen;
using StockApp.Commands;
using StockApp.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class LiveZielResultViewModel : BaseViewModel, IDialogRequestClose, IDisposable
    {
        public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
        public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;

        readonly Turnier turnier;
        readonly Zielbewerb bewerb;
        readonly NetworkService networkService;

        public LiveZielResultViewModel(Turnier turnier)
        {
            this.turnier = turnier;
            this.bewerb = turnier.Wettbewerb as Zielbewerb;
            this.networkService = NetworkService.Instance;
            this.bewerb.PropertyChanged += Bewerb_PropertyChanged;
            networkService.StartStopStateChanged += NetworkService_StartStopStateChanged;
        }

        private void NetworkService_StartStopStateChanged(object sender, NetworkServiceStateEventArgs e)
        {
            RaisePropertyChanged(nameof(IsListenerOnline));
        }

        private void Bewerb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Ergebnisliste));
        }

        public void Dispose()
        {
            this.networkService.StartStopStateChanged -= NetworkService_StartStopStateChanged;
        }

        private bool isLive;

        public bool IsLive
        {
            get => isLive;
            set
            {
                if (SetProperty(ref isLive, value))
                    RaisePropertyChanged(nameof(Ergebnisliste));
            }
        }

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

        public string WindowTitle => "StockApp Live-Ergebnis";


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

        public ObservableCollection<(int Platzierung, Teilnehmer Spieler, bool isLive)> Ergebnisliste
        {
            get
            {
                var liste = new ObservableCollection<(int _platzierung, Teilnehmer _spieler, bool _isLive)>();
                int i = 1;
                foreach (var t in bewerb.GetTeilnehmerRanked())
                {
                    liste.Add((i, t, this.IsLive));
                    i++;
                }
                return liste;
            }
        }
    }

    public class LiveZielResultDesignViewModel
    {
        public LiveZielResultDesignViewModel()
        {
            this.Ergebnisliste = new ObservableCollection<(int, Teilnehmer, bool)>
            {
                (1, new Teilnehmer(){ LastName = "Schmeckenbecher", FirstName ="Thomas"},true),
                (2, new Teilnehmer(){ LastName = "Bretterklieber", FirstName ="Franz"},true),
                (3, new Teilnehmer(){ LastName = "Hartmann", FirstName ="Paul"},true),
                (4, new Teilnehmer(){ LastName = "Schumacher ", FirstName ="Samuel"},true),
                (5, new Teilnehmer(){ LastName = "Brandt", FirstName ="hristian"},true),
                (6, new Teilnehmer(){ LastName = "Hansen", FirstName ="Stefan"},true),
                (7, new Teilnehmer(){ LastName = "Kaufmann", FirstName ="Xaver"},true),
                (8, new Teilnehmer(){ LastName = "Schilling", FirstName ="Alexander"},true),
                (9, new Teilnehmer(){ LastName = "Geiger", FirstName ="Michael"},true),
                (10, new Teilnehmer(){ LastName = "Naumann", FirstName ="Maximilian"},true),
                (11, new Teilnehmer(){ LastName = "Maurer ", FirstName ="Jonathan"},true),
                (12, new Teilnehmer(){ LastName = "Bender", FirstName ="Andreas"},true),
                (13, new Teilnehmer(){ LastName = "Böttcher", FirstName ="Julia"},true),
                (14, new Teilnehmer(){ LastName = "Petersen", FirstName ="Mara"},true),
                (15, new Teilnehmer(){ LastName = "Schreiber", FirstName ="Franziska"},true),
                (16, new Teilnehmer(){ LastName = "Winter", FirstName ="Stefanie"},true),
                (17, new Teilnehmer(){ LastName = "Krämer", FirstName ="Elisabeth"},true),
                (18, new Teilnehmer(){ LastName = "König", FirstName ="Emma"},true),
                (19, new Teilnehmer(){ LastName = "Zimmermann", FirstName ="Laura"},true),
                (20, new Teilnehmer(){ LastName = "Sommer", FirstName ="Elfriede"},true),
                (21, new Teilnehmer(){ LastName = "Müller ", FirstName ="Konstanze"},true),
            };
        }


        public ObservableCollection<(int Platzierung, Teilnehmer Spieler, bool isLive)> Ergebnisliste { get; }

        public string WindowTitle { get; } = "StockApp Live-Result";
        public ICommand CloseCommand { get; }
        public ICommand RefreshCommand { get; }

        public bool IsLive { get; set; }
    }
}

using StockApp.BaseClasses;
using StockApp.BaseClasses.Zielschiessen;
using StockApp.Commands;
using StockApp.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class ZielSpielerViewModel : BaseViewModel, IZielSpielerViewModel
    {
        private readonly Turnier _turnier;
        private readonly Zielbewerb _bewerb;
        private Teilnehmer _selectedPlayer;
        private Wertung _selectedWertung;

        /// <summary>
        /// Default-Constructor
        /// </summary>
        /// <param name="turnier"></param>
        public ZielSpielerViewModel(Turnier turnier)
        {
            _turnier = turnier;
            _bewerb = _turnier.Wettbewerb as Zielbewerb;
        }

        #region Properties

        public Teilnehmer SelectedPlayer
        {
            get => _selectedPlayer ??= Players.First();
            set => SetProperty(ref _selectedPlayer, value);
        }

        public Wertung SelectedWertung
        {
            get => _selectedWertung ??= SelectedPlayer.Wertungen.First();
            set => SetProperty(ref _selectedWertung, value);
        }


        private int _selectedZielBahn;
        public int SelectedZielBahn
        {
            get => _selectedZielBahn;
            set => SetProperty(ref _selectedZielBahn, value);
        }


        #region readonly Properties

        public ObservableCollection<Teilnehmer> Players
        {
            get
            {
                return new ObservableCollection<Teilnehmer>(_bewerb.Teilnehmerliste);
            }
        }

        public Zielbewerb Bewerb
        {
            get
            {
                return _bewerb;
            }
        }


        #endregion

        #endregion

        #region Commands

        private ICommand _addPlayerCommand;
        public ICommand AddPlayerCommand
        {
            get
            {
                return _addPlayerCommand ??= new RelayCommand(
                    (p) =>
                    {
                        _bewerb.AddNewTeilnehmer();
                        RaisePropertyChanged(nameof(this.Players));

                    },
                    (p) => _bewerb.CanAddTeilnehmer()
                    ); ;
            }
        }


        private ICommand _removePlayerCommand;
        public ICommand RemovePlayerCommand
        {
            get
            {
                return _removePlayerCommand ??= new RelayCommand(
                    (p) =>
                    {
                        _bewerb.RemoveTeilnehmer(_selectedPlayer);
                        RaisePropertyChanged(nameof(this.Players));

                    },
                    (p) => _bewerb.CanRemoveTeilnehmer()
                    );

            }
        }


        private ICommand _addWertungCommand;
        public ICommand AddWertungCommand
        {
            get
            {
                return _addWertungCommand ??= new RelayCommand(
                    (p) =>
                    {
                        SelectedPlayer.AddNewWertung();
                    },
                    (p) => SelectedPlayer.CanAddWertung()
                    );
            }
        }


        private ICommand _removeWertungCommand;
        public ICommand RemoveWertungCommand
        {
            get
            {
                return _removeWertungCommand ??= new RelayCommand(
                    (p) =>
                    {
                        SelectedPlayer.RemoveWertung(SelectedWertung);
                    },
                    (p) => (!SelectedWertung.IsOnline) && (SelectedPlayer.CanRemoveWertung())
                    );
            }
        }


        private ICommand _setWertungOnlineCommand;
        public ICommand SetWertungOnlineCommand
        {
            get
            {
                return _setWertungOnlineCommand ??= new RelayCommand(
                    (p) =>
                    {
                        if (!SelectedPlayer.HasOnlineWertung)
                        {
                            SelectedPlayer.SetAktuelleBahn(this._selectedZielBahn, SelectedWertung.Nummer);
                        }
                        else
                        {
                            SelectedPlayer.DeleteAktuellBahn();
                        }
                        SelectedZielBahn = this._bewerb.FreieBahnen.First();
                    },
                    (p) =>
                    {
                        if (SelectedPlayer.HasOnlineWertung)
                            return SelectedPlayer.Onlinewertung == SelectedWertung;
                        else
                        {
                            return SelectedZielBahn > 0 && SelectedWertung.GesamtPunkte == 0;
                        }
                    }
                    );
            }
        }

        #endregion

        #region Functions

        public void MoveTeilnehmer(int oldIndex, int newIndex)
        {
            _bewerb.MoveTeilnehmer(oldIndex, newIndex);
            RaisePropertyChanged(nameof(Players));
        }

        #endregion
    }


    public class ZielSpielerDesignViewModel : BaseViewModel, IZielSpielerViewModel
    {
        private readonly Turnier turnier = ZielBewerbExtension.GetSampleZielBewerbTurnier();

        public ZielSpielerDesignViewModel()
        {

        }


        public Zielbewerb Bewerb
        {
            get => turnier.Wettbewerb as Zielbewerb;
        }


        public Teilnehmer SelectedPlayer
        {
            get => Players.First(p => p.Startnummer == 1);
            set { }
        }

        public ObservableCollection<Teilnehmer> Players => new ObservableCollection<Teilnehmer>(Bewerb.Teilnehmerliste);

        public Wertung SelectedWertung
        {
            get => SelectedPlayer.Wertungen[0];
            set { }
        }

        public int SelectedZielBahn
        {
            get;
            set;
        }


        public ICommand AddPlayerCommand { get; }
        public ICommand RemovePlayerCommand { get; }
        public ICommand AddWertungCommand { get; }
        public ICommand RemoveWertungCommand { get; }
        public ICommand SetWertungOnlineCommand { get; }


    }
}

using StockApp.BaseClasses;
using StockApp.Commands;
using StockApp.Interfaces;
using StockApp.Output;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.ViewModels
{
    public class TeamsViewModel : BaseViewModel, ITeamsViewModel
    {
        private readonly Turnier turnier;
        private readonly TeamBewerb teamBewerb;
        public TeamsViewModel(Turnier turnier)
        {
            this.turnier = turnier;
            teamBewerb = turnier.Wettbewerb as TeamBewerb;
        }

        public ObservableCollection<Team> Teams => new(teamBewerb.Teams.Where(t => !t.IsVirtual));

        public ObservableCollection<Player> Players
        {
            get
            {
                if (SelectedTeam == null) return null;
                return new ObservableCollection<Player>(SelectedTeam.Players);
            }
        }

        private Team _selectedTeam;
        public Team SelectedTeam
        {
            get => _selectedTeam ??= Teams.FirstOrDefault();
            set
            {
                if (SetProperty(ref _selectedTeam, value))
                    RaisePropertyChanged(nameof(Players));
            }
        }

        private Player _selectedPlayer;
        public Player SelectedPlayer
        {
            get => _selectedPlayer ??= SelectedTeam?.Players.FirstOrDefault();
            set => SetProperty(ref _selectedPlayer, value);
        }

        private ICommand _removeTeamCommand;
        public ICommand RemoveTeamCommand
        {
            get
            {
                return _removeTeamCommand ??= new RelayCommand(
                    (p) =>
                    {
                        teamBewerb.RemoveTeam(SelectedTeam);
                        RaisePropertyChanged(nameof(Teams));
                    },
                (o) =>
                {
                    return SelectedTeam != null;
                }
                );
            }
        }

        private ICommand _addTeamCommand;
        public ICommand AddTeamCommand
        {
            get
            {
                return _addTeamCommand ??= new RelayCommand(
                    (p) =>
                    {
                        teamBewerb.AddTeam(new Team($"default {teamBewerb.Teams.Count + 1}"), true);
                        RaisePropertyChanged(nameof(Teams));
                    },
                    (p) =>
                    {
                        return Teams.Count(t => !t.IsVirtual) < 15;
                    });
            }
        }

        private ICommand printQuittungenCommand;
        public ICommand PrintQuittungenCommand
        {
            get
            {
                return printQuittungenCommand ??= new RelayCommand(
                    (p) =>
                    {
                        var x = new Output.Receipts.Receipt(turnier);
                        PrintPreview printPreview = new PrintPreview();
                        var A4Size = new System.Windows.Size(8 * 96, 11.5 * 96);
                        printPreview.Document = x.CreateReceipts(A4Size);
                        printPreview.ShowDialog();
                    },
                    (p) => Teams.Count > 0
                    );
            }
        }

        private ICommand addPlayerCommand;
        public ICommand AddPlayerCommand
        {
            get
            {
                return addPlayerCommand ??= new RelayCommand(
                    (p) =>
                    {
                        SelectedTeam.AddPlayer();
                        RaisePropertyChanged(nameof(Players));

                    },
                    (p) =>
                     Players?.Count < Team.MaxNumberOfPlayers);
            }
        }

        private ICommand removePlayerCommand;
        public ICommand RemovePlayerCommand
        {
            get
            {
                return removePlayerCommand ??= new RelayCommand(
                    (p) =>
                    {
                        SelectedTeam.RemovePlayer(SelectedPlayer);
                        RaisePropertyChanged(nameof(Players));
                    },
                    (p) =>
                    Players?.Count > Team.MinNumberOfPlayer
                    && SelectedPlayer != null);
            }
        }
    }

    public class TeamsDesignViewModel : ITeamsViewModel
    {
        private readonly TeamBewerb t = TeamBewerbExtension.CreateNewTournament(true);
        public TeamsDesignViewModel()
        {
            SelectedTeam = t.Teams[3];
            Teams = new ObservableCollection<Team>(t.Teams);
        }

        public ObservableCollection<Team> Teams { get; }
        public ObservableCollection<Player> Players
        {
            get
            {
                return new ObservableCollection<Player>(SelectedTeam.Players);
            }
        }

        public Team SelectedTeam { get; set; }
        public Player SelectedPlayer { get; set; }
        public ICommand AddTeamCommand { get; }
        public ICommand RemoveTeamCommand { get; }
        public ICommand AddPlayerCommand { get; }
        public ICommand RemovePlayerCommand { get; }
        public ICommand PrintQuittungenCommand { get; }
    }
}

using StockApp.BaseClasses;
using System.Collections.ObjectModel;

namespace StockApp.Interfaces
{

    public interface IResultsViewModel
    {
        ObservableCollection<Team> Teams { get; }
        ObservableCollection<Game> Games { get; }
        Team SelectedTeam { get; set; }
        Game SelectedGame { get; set; }
        ObservableCollection<PointsPerTeamAndGame> PointsOfSelectedTeam { get; }
        ObservableCollection<PointsPerGame> PointsPerGameList { get; set; }
        int NumberOfTeamsWithNamedPlayers { get; set; }

    } //IResultsViewModel
}
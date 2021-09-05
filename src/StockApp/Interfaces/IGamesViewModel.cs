using StockApp.BaseClasses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.Interfaces
{
    public interface IGamesViewModel
    {
        int NumberOfCourts { get; }
        int NumberOfGameRounds { get; set; }
        ObservableCollection<Team> Teams { get; }
        bool TwoPauseGames { get; set; }
        bool ConcatRoundsOnOutput { get; set; }
        bool TeamNameOnTurnCards { get; set; }
        bool IsDirectionOfCourtsFromRightToLeft { get; set; }
        string DirectionOfCourtsDescription { get; }
        bool IsStartOfGameChanged { get; set; }
        bool Is8KehrenSpiel { get; set; }
        bool IsTurnCardForStockTV { get; set; }
        
       
        ICommand CreateGamesCommand { get; }
        ICommand SendTeamsToStockTVCommand { get; }
        ICommand PrintTurnCardsCommand { get; }
        ICommand PrintBahnblockCommand { get; }
    }
}

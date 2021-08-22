using StockApp.BaseClasses;
using System.Collections.ObjectModel;

namespace StockApp.Interfaces
{

    internal interface ITeamBewerb 
    {
        /// <summary>
        /// Liste aller Teams
        /// </summary>
        ReadOnlyCollection<Team> Teams { get; }

        /// <summary>
        /// Anzahl der Stockbahnen / Spielfächen
        /// </summary>
        int NumberOfCourts { get; }


        /// <summary>
        /// Number of rounds to play (default 1) 
        /// </summary>
        int NumberOfGameRounds { get; set; }


        /// <summary>
        /// True, if Court#1 is on the right. Team with StartNumber 1 is also on right side and goes to left for 2nd game (default True)
        /// </summary>
        bool IsDirectionOfCourtsFromRightToLeft { get; set; }

        /// <summary>
        /// If true, on even number of Teams, there are two games as pause
        /// </summary>
        bool TwoPauseGames { get; set; }


        /// <summary>
        /// On True, the TurnCard has 8 instead of 7 Turns per Team
        /// </summary>
        bool Is8TurnsGame { get; set; }

        /// <summary>
        /// If true, the Game-Start is switched after every round of game
        /// </summary>
        bool StartingTeamChange { get; set; }

        /// <summary>
        /// Number of Top Teams, where the names of the players are on the printed Result
        /// </summary>
        int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

        
    }
}


using StockApp.BaseClasses;
using System;
using System.Collections.ObjectModel;

namespace StockApp.Interfaces
{
    internal interface ITournament
    {

        /// <summary>
        /// Liste aller Teams
        /// </summary>
        ReadOnlyCollection<Team> Teams { get;  }

        /// <summary>
        /// Veranstaltungsort
        /// </summary>
        string Venue { get; set; }

        /// <summary>
        /// Organisator / Veranstalter
        /// </summary>
        string Organizer { get; set; }

        /// <summary>
        /// Datum / Zeit des Turniers
        /// </summary>
        DateTime DateOfTournament { get; set; }

        /// <summary>
        /// Durchführer
        /// </summary>
        string Operator { get; set; }

        /// <summary>
        /// Turniername
        /// </summary>
        string TournamentName { get; set; }

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
        /// EntryFee per Team
        /// </summary>
        EntryFee EntryFee { get; set; }

        /// <summary>
        /// Number of Top Teams, where the names of the players are on the printed Result
        /// </summary>
        int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

        /// <summary>
        /// Spielgruppe im Turnier, wenn mehrere Turniere gleichzeitig stattfinden. 
        /// 
        /// Default 0: Wenn nur eine Gruppe vorhanden ist
        /// </summary>
        public int SpielGruppe { get; set; }

        /// <summary>
        /// Referee
        /// </summary>
        Referee Referee { get; set; }

        /// <summary>
        /// CompetitionManager
        /// </summary>
        CompetitionManager CompetitionManager { get; set; }

        /// <summary>
        /// ComputingOfficer
        /// </summary>
        ComputingOfficer ComputingOfficer { get; set; }
    }
}

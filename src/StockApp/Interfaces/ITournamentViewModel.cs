using StockApp.BaseClasses;
using System;

namespace StockApp.Interfaces
{
    public interface ITournamentViewModel
    {
        string Venue { get; set; }
        string Organizer { get; set; }
        string Operator { get; set; }
        string TournamentName { get; set; }
        DateTime DateOfTournament { get; set; }
        EntryFee EntryFee { get; set; }

        int SpielGruppe { get; set; }
       
        Referee Referee { get; set; }
        CompetitionManager CompetitionManager { get; set; }
        ComputingOfficer ComputingOfficer { get; set; }
    }
}

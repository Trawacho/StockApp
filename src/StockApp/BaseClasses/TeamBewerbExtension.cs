using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    internal class TeamBewerbExtension
    {
        public static TeamBewerb CreateNewTournament(bool generate_9_Teams = false)
        {
            var tournament = new TeamBewerb()
            {
                NumberOfGameRounds = 1,
                TwoPauseGames = false,
                //EntryFee = new EntryFee(30.00, "dreißig"),
                //Organizer = "Eisstockfreunde Hankofen",
                //DateOfTournament = DateTime.Now,
                //Operator = "Kreis 105 Gäuboden/Vorwald",
                //TournamentName = "1. Stockturnier Herren 2020",
                //Venue = "Hankofen"
            };

            tournament.AddTeam(new Team("ESF Hankofen"));
            tournament.AddTeam(new Team("EC Pilsting"));
            tournament.AddTeam(new Team("DJK Leiblfing"));
            tournament.AddTeam(new Team("ETSV Hainsbach"));
            tournament.AddTeam(new Team("SV Salching"));
            tournament.AddTeam(new Team("SV Haibach"));
            tournament.AddTeam(new Team("TSV Bogen"));
            tournament.AddTeam(new Team("EC EBRA Aiterhofen"));
            if (generate_9_Teams)
                tournament.AddTeam(new Team("EC Welchenberg"));

            tournament.CreateGames();

            return tournament;

        }


      


    }
}

using StockApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class SerializableTournamentSet : ITournament
    {
        public SerializableTournamentSet()
        {

        }

        public void SetTournament(Tournament tournament)
        {
            this.XTeams = tournament.Teams.ToList();
            this.Games = tournament.GetAllGames()
                                 .OrderBy(r => r.RoundOfGame)
                                 .ThenBy(g => g.GameNumber)
                                 .ThenBy(c => c.CourtNumber)
                                 .ToList();
            this.TournamentName = tournament.TournamentName;
            this.Venue = tournament.Venue;
            this.Operator = tournament.Operator;
            this.Organizer = tournament.Organizer;
            this.DateOfTournament = tournament.DateOfTournament;
            this.EntryFee = tournament.EntryFee;
            this.StartingTeamChange = tournament.StartingTeamChange;
            this.Is8TurnsGame = tournament.Is8TurnsGame;
            this.IsDirectionOfCourtsFromRightToLeft = tournament.IsDirectionOfCourtsFromRightToLeft;
            this.TwoPauseGames = tournament.TwoPauseGames;
            this.NumberOfGameRounds = tournament.NumberOfGameRounds;
            this.NumberOfTeamsWithNamedPlayerOnResult = tournament.NumberOfTeamsWithNamedPlayerOnResult;
            this.ComputingOfficer = tournament.ComputingOfficer;
            this.Referee = tournament.Referee;
            this.CompetitionManager = tournament.CompetitionManager;
            this.SpielGruppe = tournament.SpielGruppe;
        }


        public Tournament GetTournament()
        {
            Tournament tournament = new Tournament
            {
                Venue = this.Venue,
                Operator = this.Operator,
                Organizer = this.Organizer,
                DateOfTournament = this.DateOfTournament,
                EntryFee = this.EntryFee,
                Is8TurnsGame = this.Is8TurnsGame,
                IsDirectionOfCourtsFromRightToLeft = this.IsDirectionOfCourtsFromRightToLeft,
                TwoPauseGames = this.TwoPauseGames,
                NumberOfGameRounds = this.NumberOfGameRounds,
                NumberOfTeamsWithNamedPlayerOnResult = this.NumberOfTeamsWithNamedPlayerOnResult,
                ComputingOfficer = this.ComputingOfficer,
                Referee = this.Referee,
                StartingTeamChange = this.StartingTeamChange,
                TournamentName = this.TournamentName,
                CompetitionManager = this.CompetitionManager,
                SpielGruppe = this.SpielGruppe
            };

            tournament.RemoveAllTeams();

            foreach (var team in XTeams)
            {
                tournament.AddTeam(team);
            }

            foreach (var game in Games)
            {
                game.TeamA = XTeams.First(t => t.StartNumber == game.StartNumberTeamA);
                game.TeamB = XTeams.First(t => t.StartNumber == game.StartNumberTeamB);

                tournament.Teams.First(t => t == game.TeamA).AddGame(game);
                tournament.Teams.First(t => t == game.TeamB).AddGame(game);
            }

            return tournament;
        }

        [XmlIgnore()]
        [Obsolete ("not available in serialization", true)]
        public ReadOnlyCollection<Team> Teams
        {
            get
            {
                throw new Exception("Not allowd in Serialization");
            }
        }

        [XmlIgnore()]
        [Obsolete ("not available in serialization", true)]
        public int NumberOfCourts
        {
            get
            {
                throw new Exception("Not allowed in Serialization");
            }
        }


        [XmlElement(Order = 2)]
        public string TournamentName { get; set; }

        [XmlElement(Order = 3)]
        public string Venue { get; set; }

        [XmlElement(Order = 4)]
        public string Operator { get; set; }

        [XmlElement(Order = 5)]
        public string Organizer { get; set; }

        [XmlElement(Order = 6)]
        public DateTime DateOfTournament { get; set; }

        [XmlElement(Order = 7)]
        public EntryFee EntryFee { get; set; }

        [XmlElement(Order = 8)]
        public bool StartingTeamChange { get; set; }

        [XmlElement(Order = 9)]
        public bool Is8TurnsGame { get; set; }

        [XmlElement(Order = 10)]
        public bool IsDirectionOfCourtsFromRightToLeft { get; set; }

        [XmlElement(Order = 11)]
        public bool TwoPauseGames { get; set; }

        [XmlElement(Order = 12)]
        public int NumberOfGameRounds { get; set; }

        [XmlElement(Order = 13)]
        public int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

        [XmlElement(Order = 14)]
        public ComputingOfficer ComputingOfficer { get; set; }

        [XmlElement(Order = 15)]
        public Referee Referee { get; set; }

        [XmlElement(Order = 16)]
        public CompetitionManager CompetitionManager { get; set; }

        [XmlElement(Order = 17)]
        public int SpielGruppe { get; set; }


        [XmlArray(ElementName = "Teams", Order = 90)]
        [XmlArrayItem(nameof(Team))]
        public List<Team> XTeams { get; set; }

        [XmlArray(ElementName = nameof(Games), Order = 100)]
        [XmlArrayItem(nameof(Game))]
        public List<Game> Games { get; set; }


    }
}

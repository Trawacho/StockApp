using StockApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class SerializableTurnier : ITeamBewerb
    {
        public SerializableTurnier()
        {

        }

        public void SetTurnier(Turnier turnier)
        {
            var tournament = turnier.Wettbewerb as TeamBewerb;

            this.XTeams = tournament.Teams.ToList();
            this.Games = tournament.GetAllGames()
                                 .OrderBy(r => r.RoundOfGame)
                                 .ThenBy(g => g.GameNumber)
                                 .ThenBy(c => c.CourtNumber)
                                 .ToList();
            this.TournamentName = turnier.OrgaDaten.TournamentName;
            this.Venue = turnier.OrgaDaten.Venue;
            this.Operator = turnier.OrgaDaten.Operator;
            this.Organizer = turnier.OrgaDaten.Organizer;
            this.DateOfTournament = turnier.OrgaDaten.DateOfTournament;
            this.EntryFee = turnier.OrgaDaten.EntryFee;
            this.SpielGruppe = turnier.SpielGruppe;
            this.StartingTeamChange = tournament.StartingTeamChange;
            this.Is8TurnsGame = tournament.Is8TurnsGame;
            this.IsDirectionOfCourtsFromRightToLeft = tournament.IsDirectionOfCourtsFromRightToLeft;
            this.TwoPauseGames = tournament.TwoPauseGames;
            this.NumberOfGameRounds = tournament.NumberOfGameRounds;
            this.NumberOfTeamsWithNamedPlayerOnResult = tournament.NumberOfTeamsWithNamedPlayerOnResult;
            this.ComputingOfficer = turnier.OrgaDaten.ComputingOfficer;
            this.Referee = turnier.OrgaDaten.Referee;
            this.CompetitionManager = turnier.OrgaDaten.CompetitionManager;
        }


        public Turnier GetTurnier()
        {
            Turnier turnier = new Turnier();
            turnier.OrgaDaten.Venue = this.Venue;
            turnier.OrgaDaten.Operator = this.Operator;
            turnier.OrgaDaten.Organizer = this.Organizer;
            turnier.OrgaDaten.DateOfTournament = this.DateOfTournament;
            turnier.OrgaDaten.EntryFee = this.EntryFee;
            turnier.OrgaDaten.ComputingOfficer = this.ComputingOfficer;
            turnier.OrgaDaten.Referee = this.Referee;
            turnier.OrgaDaten.TournamentName = this.TournamentName;
            turnier.OrgaDaten.CompetitionManager = this.CompetitionManager;
            turnier.SpielGruppe = this.SpielGruppe;

            TeamBewerb teambewerb = new TeamBewerb
            {
                Is8TurnsGame = this.Is8TurnsGame,
                IsDirectionOfCourtsFromRightToLeft = this.IsDirectionOfCourtsFromRightToLeft,
                TwoPauseGames = this.TwoPauseGames,
                NumberOfGameRounds = this.NumberOfGameRounds,
                NumberOfTeamsWithNamedPlayerOnResult = this.NumberOfTeamsWithNamedPlayerOnResult,
                StartingTeamChange = this.StartingTeamChange,
            };

            teambewerb.RemoveAllTeams();

            foreach (var team in XTeams)
            {
                teambewerb.AddTeam(team);
            }

            foreach (var game in Games)
            {
                game.TeamA = XTeams.First(t => t.StartNumber == game.StartNumberTeamA);
                game.TeamB = XTeams.First(t => t.StartNumber == game.StartNumberTeamB);

                teambewerb.Teams.First(t => t == game.TeamA).AddGame(game);
                teambewerb.Teams.First(t => t == game.TeamB).AddGame(game);
            }

            turnier.Wettbewerb = teambewerb;
            return turnier;
        }

        [XmlIgnore()]
        [Obsolete("not available in serialization", true)]
        public ReadOnlyCollection<Team> Teams
        {
            get
            {
                throw new Exception("Not allowd in Serialization");
            }
        }

        [XmlIgnore()]
        [Obsolete("not available in serialization", true)]
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
        public Startgebuehr EntryFee { get; set; }

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
        public Rechenbuero ComputingOfficer { get; set; }

        [XmlElement(Order = 15)]
        public Schiedsrichter Referee { get; set; }

        [XmlElement(Order = 16)]
        public Wettbewerbsleiter CompetitionManager { get; set; }

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

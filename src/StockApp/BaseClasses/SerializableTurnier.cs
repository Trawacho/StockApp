using StockApp.BaseClasses.Zielschiessen;
using StockApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class SerializableTurnier : ITeamBewerb, IZielbewerb
    {
        public SerializableTurnier()
        {

        }

        public void SetTurnier(Turnier turnier)
        {

            this.Organisation = turnier.OrgaDaten;

            this.Wettbewerbsart = turnier.Wettbewerb.GetType().Name;

            if (turnier.Wettbewerb is TeamBewerb teamBewerb)
            {
                this.XTeams = teamBewerb.Teams.ToList();
                this.Games = teamBewerb.GetAllGames()
                                     .OrderBy(r => r.RoundOfGame)
                                     .ThenBy(g => g.GameNumber)
                                     .ThenBy(c => c.CourtNumber)
                                     .ToList();

                this.SpielGruppe = teamBewerb.SpielGruppe;
                this.StartingTeamChange = teamBewerb.StartingTeamChange;
                this.Is8TurnsGame = teamBewerb.Is8TurnsGame;
                this.IsDirectionOfCourtsFromRightToLeft = teamBewerb.IsDirectionOfCourtsFromRightToLeft;
                this.TwoPauseGames = teamBewerb.TwoPauseGames;
                this.NumberOfGameRounds = teamBewerb.NumberOfGameRounds;
                this.NumberOfTeamsWithNamedPlayerOnResult = teamBewerb.NumberOfTeamsWithNamedPlayerOnResult;
            }
            else if (turnier.Wettbewerb is Zielbewerb zielbewerb)
            {
                this.XTeilnehmerliste = new List<XMLTeilnehmer>();
                foreach (var t in zielbewerb.Teilnehmerliste)
                {
                    this.XTeilnehmerliste.Add(new XMLTeilnehmer(t));
                }
            }
        }


        public Turnier GetTurnier()
        {
            Turnier turnier = new Turnier
            {
                OrgaDaten = this.Organisation
            };



            if (Wettbewerbsart == nameof(TeamBewerb))
            {
                turnier.SetBewerb(BaseClasses.Wettbewerbsart.Team);
                TeamBewerb teambewerb = turnier.Wettbewerb as TeamBewerb;
                teambewerb.Is8TurnsGame = this.Is8TurnsGame;
                teambewerb.IsDirectionOfCourtsFromRightToLeft = this.IsDirectionOfCourtsFromRightToLeft;
                teambewerb.TwoPauseGames = this.TwoPauseGames;
                teambewerb.NumberOfGameRounds = this.NumberOfGameRounds;
                teambewerb.NumberOfTeamsWithNamedPlayerOnResult = this.NumberOfTeamsWithNamedPlayerOnResult;
                teambewerb.StartingTeamChange = this.StartingTeamChange;
                teambewerb.SpielGruppe = this.SpielGruppe;

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

            }
            else if (Wettbewerbsart == nameof(Zielbewerb))
            {
                turnier.SetBewerb(BaseClasses.Wettbewerbsart.Ziel);
                var bewerb = turnier.Wettbewerb as Zielbewerb;

                foreach (var t in this.XTeilnehmerliste)
                {
                    var tln = new Teilnehmer()
                    {
                        FirstName = t.Vorname,
                        LastName = t.Nachname,
                        Vereinsname = t.Vereinsname,
                        Nation = t.Nation,
                        LicenseNumber = t.Passnummer,
                        Startnummer = t.Startnummer
                    };
                    foreach (var wertung in t.Wertungen)
                    {
                        var w = new Wertung() { Nummer = wertung.Nummer };
                        foreach (var v in wertung.Werte)
                        {
                            w.AddVersuch(v);
                        }
                    }

                    bewerb.AddTeilnehmer(tln);
                }

            }
            return turnier;
        }





        #region OrgaDaten

        [XmlElement(Order = 1)]
        public OrgaDaten Organisation { get; set; }

        [XmlElement(Order = 3)]
        public string Wettbewerbsart { get; set; }

        #endregion

        #region TeamBewerb
        [XmlElement(Order = 10)]
        public int SpielGruppe { get; set; }

        [XmlElement(Order = 11)]
        public bool StartingTeamChange { get; set; }

        [XmlElement(Order = 12)]
        public bool Is8TurnsGame { get; set; }

        [XmlElement(Order = 13)]
        public bool IsDirectionOfCourtsFromRightToLeft { get; set; }

        [XmlElement(Order = 14)]
        public bool TwoPauseGames { get; set; }

        [XmlElement(Order = 15)]
        public int NumberOfGameRounds { get; set; }

        [XmlElement(Order = 16)]
        public int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

        [XmlArray(ElementName = "Teams", Order = 17)]
        [XmlArrayItem(nameof(Team))]
        public List<Team> XTeams { get; set; }

        [XmlArray(ElementName = nameof(Games), Order = 18)]
        [XmlArrayItem(nameof(Game))]
        public List<Game> Games { get; set; }


        #region unused

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

        public string SpielGruppeString()
        {
            throw new NotImplementedException();
        }

        #endregion



        #endregion


        #region ZielBewerb

        [XmlArrayItem(nameof(Teilnehmer))]
        [XmlArray(ElementName = nameof(Teilnehmerliste), Order = 200)]
        public List<XMLTeilnehmer> XTeilnehmerliste { get; set; }






        #region unused
        [XmlIgnore]
        public IOrderedEnumerable<Teilnehmer> Teilnehmerliste => throw new NotImplementedException();

        [XmlIgnore]
        public IOrderedEnumerable<int> Bahnen => throw new NotImplementedException();

        [XmlIgnore]
        public IOrderedEnumerable<int> FreieBahnen => throw new NotImplementedException();

        public void AddNewTeilnehmer()
        {
            throw new NotImplementedException();
        }

        public bool CanAddTeilnehmer()
        {
            throw new NotImplementedException();
        }

        public void RemoveTeilnehmer(Teilnehmer teilnehmer)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveTeilnehmer()
        {
            throw new NotImplementedException();
        }

        public void MoveTeilnehmer(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }


        #endregion

        #endregion
    }

    public class XMLTeilnehmer
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Nation { get; set; }
        public string Passnummer { get; set; }
        public int Startnummer { get; set; }
        public string Vereinsname { get; set; }
        public List<XMLZielbewerbWertung> Wertungen { get; set; }
        public XMLTeilnehmer(Teilnehmer t)
        {
            this.Vorname = t.FirstName;
            this.Nachname = t.LastName;
            this.Nation = t.Nation;
            this.Passnummer = t.LicenseNumber;
            this.Startnummer = t.Startnummer;
            this.Vereinsname = t.Vereinsname;
            Wertungen = new List<XMLZielbewerbWertung>();
            foreach (var w in t.Wertungen)
            {
                Wertungen.Add(new XMLZielbewerbWertung(w));
            }

        }
        public XMLTeilnehmer()
        {

        }
    }

    public class XMLZielbewerbWertung
    {
        public List<int> Werte { get; set; }
        public int Nummer { get; set; }

        public XMLZielbewerbWertung()
        {

        }

        public XMLZielbewerbWertung(Wertung wertung)
        {
            Nummer = wertung.Nummer;
            Werte = new List<int>();
            foreach (var d in wertung.Disziplinen)
            {
                foreach (var w in d.GetVersuche())
                {
                    Werte.Add(w);
                }
            }
        }
    }


}

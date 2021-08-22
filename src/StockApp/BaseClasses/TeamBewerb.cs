using StockApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.BaseClasses
{
    public class TeamBewerb : TBaseBewerb, ITeamBewerb
    {
        #region Fields

        private readonly List<Team> _teams;
        private bool _twoPauseGames;
        private int _numberOfGameRounds;

        #endregion

        #region Properties

        /// <summary>
        /// Liste aller Teams
        /// </summary>
        public ReadOnlyCollection<Team> Teams { get; }

        /// <summary>
        /// Anzahl der Stockbahnen / Spielfächen
        /// </summary>
        public int NumberOfCourts=>  Teams.Count(t => !t.IsVirtual) / 2; 

        /// <summary>
        /// Number of rounds to play (default 1) 
        /// </summary>
        public int NumberOfGameRounds
        {
            get
            {
                return _numberOfGameRounds;
            }
            set
            {
                if (_numberOfGameRounds == value) return;
                if (value < 1 || value > 7) return;

                _numberOfGameRounds = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// True, if Court#1 is on the right. Team with StartNumber 1 is also on right side and goes to left for 2nd game (default True)
        /// </summary>
        public bool IsDirectionOfCourtsFromRightToLeft { get; set; }

        /// <summary>
        /// True, if every Team has two pause
        /// </summary>
        public bool TwoPauseGames
        {
            get
            {
                return Teams.Count(t => !t.IsVirtual) % 2 == 0
                                        ? _twoPauseGames
                                        : false;
            }
            set
            {
                if (value == _twoPauseGames) return;
                _twoPauseGames = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// On True, the TurnCard has 8 instead of 7 Turns per Team
        /// </summary>
        public bool Is8TurnsGame { get; set; }

        /// <summary>
        /// True, wenn bei einer Mehrfachrunde das Anspiel bei jeder Runde gewechselt wird
        /// </summary>
        public bool StartingTeamChange { get; set; }

        /// <summary>
        /// Bei wieviel Mannschaften werden die Spielernamen auf der Ergebnisliste mit angedruckt
        /// </summary>
        public int NumberOfTeamsWithNamedPlayerOnResult { get; set; }

       

        #endregion

        #region Constructor

        public TeamBewerb()
        {
            this.IsDirectionOfCourtsFromRightToLeft = true;
            this.NumberOfGameRounds = 1;
            this.TwoPauseGames = false;
            this.Is8TurnsGame = false;

            this.StartingTeamChange = false;

            this._teams = new List<Team>();
            this.Teams = _teams.AsReadOnly();
            this.NumberOfTeamsWithNamedPlayerOnResult = 3;

        }

        #endregion

        #region Functions

        /// <summary>
        /// Alle Spiele aller Mannschaften
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Game> GetAllGames()
        {
            return Teams.SelectMany(g => g.Games).Distinct();
        }

        /// <summary>
        /// Anzahl aller Spiele aller Mannschaften
        /// </summary>
        /// <returns></returns>
        internal int CountOfGames()
        {
            return GetAllGames().ToList().Count();
        }

        /// <summary>
        /// Alle Spiele auf der Bahn x
        /// </summary>
        /// <param name="courtNumber"></param>
        /// <returns></returns>
        public IEnumerable<Game> GetGamesOfCourt(int courtNumber)
        {
            return Teams.SelectMany(g => g.Games)
                .Distinct()
                .Where(c => c.CourtNumber == courtNumber)
                .OrderBy(r => r.RoundOfGame)
                .ThenBy(s => s.GameNumber);
        }

        /// <summary>
        /// Platzierungs Liste der Mannschaften 
        /// </summary>
        /// <param name="live"></param>
        /// <returns></returns>
        public IEnumerable<Team> GetTeamsRanked(bool live = false)
        {
            return live
                        ? Teams
                            .Where(v => !v.IsVirtual)
                            .OrderByDescending(t => t.SpielPunkte_LIVE.positiv)
                            .ThenByDescending(p => p.StockNote_LIVE)
                            .ThenByDescending(d => d.StockPunkteDifferenz_LIVE)
                        : Teams
                            .Where(v => !v.IsVirtual)
                            .OrderByDescending(t => t.SpielPunkte.positiv)
                            .ThenByDescending(p => p.StockNote)
                            .ThenByDescending(d => d.StockPunkteDifferenz);
        }

        /// <summary>
        /// Es wird in allen Spielen die Master- und NetworkTurns auf 0:0 gesetzt
        /// </summary>
        internal void ResetAllGames()
        {
            Parallel.ForEach(GetAllGames(), (g) =>
            {
                g.NetworkTurn.Reset();
                g.MasterTurn.Reset();
            });
        }


        #endregion

        #region Team Functions

        public void AddVirtualTeams(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddTeam(new Team("Virtual Team")
                {
                    IsVirtual = true
                },
                i == 0);
            }
        }

        internal void RemoveAllVirtualTeams()
        {
            _teams.RemoveAll(t => t.IsVirtual);
            RaisePropertyChanged(nameof(Teams));
        }

        internal void RemoveAllTeams()
        {
            _teams.Clear();
            RaisePropertyChanged(nameof(Teams));
        }


        /// <summary>
        /// Adds a Team to the Tournament.
        /// - All Games were deleted
        /// - Startnumbers were reOrganized
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to add</param>
        /// <param name="deleteVirtualTeamsFirst">If TRUE, all VirtualTeams are removed first</param>
        public void AddTeam(Team team, bool deleteVirtualTeamsFirst = false)
        {
            if (deleteVirtualTeamsFirst)
                RemoveAllVirtualTeams();

            Parallel.ForEach(_teams, (t) => t.ClearGames());

            ReOrganizeTeamStartNumbers();

            team.StartNumber = _teams.Count + 1;

            this._teams.Add(team);

            RaisePropertyChanged(nameof(Teams));

        }

        /// <summary>
        /// Removes the Team from the Tournament
        /// - Remove alle Virtual Teams
        /// - Startnumbers were reOrganized
        /// - all Games were deleted
        /// </summary>
        /// <param name="team"></param>
        public void RemoveTeam(Team team)
        {
            RemoveAllVirtualTeams();
            this._teams.Remove(team);

            Parallel.ForEach(_teams, (t) => t.ClearGames());

            ReOrganizeTeamStartNumbers();

            RaisePropertyChanged(nameof(Teams));
        }

        /// <summary>
        /// ReOrganize Startnumbers of each Team in List as index-based
        /// </summary>
        private void ReOrganizeTeamStartNumbers()
        {
            for (int i = 0; i < _teams.Count; i++)
            {
                _teams[i].StartNumber = i + 1;
            }
        }

        #endregion

        /// <summary>
        /// Removes all Games und Creates new Games
        /// </summary>
        internal void ReCreateGames()
        {
            RemoveAllGames();
            CreateGames();
        }

        /// <summary>
        /// Removes all Games from every Team and also removes the virtual Teams
        /// </summary>
        internal void RemoveAllGames()
        {
            Parallel.ForEach(Teams, (t) => t.ClearGames());
            RemoveAllVirtualTeams();
        }

        /// <summary>
        /// Funktion zur Erzeugung des Spielplans
        /// </summary>
        internal void CreateGames()
        {
            /*
             *  Auf dieser Seite findet man Informationen bzgl der Berchnung eines Spielplans
             *  http://www-i1.informatik.rwth-aachen.de/~algorithmus/algo36.php
             * 
             */
            //Remove Virtual Teams to check the number of VirtualTeams needed
            RemoveAllVirtualTeams();

            int iBahnCor = 0;               //Korrektur-Wert für Bahn

            //Bei ungerade Zahl an Teams ein virtuelles Team hinzufügen
            if (Teams.Count % 2 == 1)
            {
                AddVirtualTeams(1);
            }
            else
            {
                //Gerade Anzahl an Mannschaften
                //Entweder kein Aussetzer oder ZWEI Aussetzer
                //if (NumberOfPauseGames == 2)
                if (TwoPauseGames)
                {
                    AddVirtualTeams(2);
                }
            }

            System.Diagnostics.Debug.WriteLine($"Schleifenstart");
            for (int spielRunde = 1; spielRunde <= this.NumberOfGameRounds; spielRunde++)
            {

                //Über Schleifen die Spiele erstellen, Teams, Bahnen und Anspiel festlegen
                for (int i = 1; i < Teams.Count; i++)
                {
                    Game game = new Game
                    {
                        TeamB = Teams.First(t => t.StartNumber == Teams.Count),
                        TeamA = Teams.First(t => t.StartNumber == i),
                        GameNumber = Teams.Count - i,
                        RoundOfGame = spielRunde,
                        GameNumberOverAll = (spielRunde - 1) * (Teams.Count - 1) + (Teams.Count - i)
                    };

                    #region Bahn Berechnen

                    if (game.IsPauseGame)
                    {
                        game.CourtNumber = 0;
                    }
                    else
                    {
                        if (i <= Teams.Count / 2)
                        {
                            game.CourtNumber = (Teams.Count / 2) - i + 1;
                        }
                        else
                        {
                            game.CourtNumber = i - (Teams.Count / 2) + 1;
                        }
                        iBahnCor = game.CourtNumber;
                    }

                    #endregion

                    #region Anspiel festlegen

                    game.IsTeamA_Starting = !(i % 2 == 0);

                    if (spielRunde == 2 && StartingTeamChange)
                    {
                        game.IsTeamA_Starting = !game.IsTeamA_Starting;
                    }



                    #endregion

                    System.Diagnostics.Debug.WriteLine(game.ToString());

                    game.TeamA.AddGame(game);
                    game.TeamB.AddGame(game);

                    for (int k = 1; k <= (Teams.Count / 2 - 1); k++)
                    {
                        game = new Game
                        {
                            GameNumber = Teams.Count - i,
                            RoundOfGame = spielRunde,
                            GameNumberOverAll = (spielRunde - 1) * (Teams.Count - 1) + (Teams.Count - i)
                        };

                        #region Team A festlegen

                        if ((i + k) % (Teams.Count - 1) == 0)
                        {
                            game.TeamA = Teams.First(t => t.StartNumber == (Teams.Count - 1));
                        }
                        else
                        {
                            var nrTb = (i + k) % (Teams.Count - 1);
                            if (nrTb < 0)
                            {
                                nrTb = (Teams.Count - 1) + nrTb;
                            }
                            game.TeamA = Teams.First(t => t.StartNumber == nrTb);
                        }

                        #endregion

                        #region Team B festlegen

                        if ((i - k) % (Teams.Count - 1) == 0)
                        {
                            game.TeamB = Teams.First(t => t.StartNumber == (Teams.Count - 1));
                        }
                        else
                        {
                            var nrTa = (i - k) % (Teams.Count - 1);
                            if (nrTa < 0)
                            {
                                nrTa = Teams.Count - 1 + nrTa;
                            }
                            game.TeamB = Teams.First(t => t.StartNumber == nrTa);

                        }

                        #endregion

                        #region Bahn berechnen

                        if (game.IsPauseGame)
                        {
                            game.CourtNumber = 0;
                        }
                        else
                        {
                            if (iBahnCor != k)
                            {
                                game.CourtNumber = k;
                            }
                            else
                            {
                                game.CourtNumber = Teams.Count / 2;
                            }
                        }

                        #endregion

                        #region Anspiel berechnen  

                        game.IsTeamA_Starting = !(k % 2 == 0);

                        if (spielRunde == 2 && StartingTeamChange)
                        {
                            game.IsTeamA_Starting = !game.IsTeamA_Starting;
                        }

                        #endregion

                        System.Diagnostics.Debug.WriteLine(game.ToString());

                        game.TeamA.AddGame(game);
                        game.TeamB.AddGame(game);
                    }
                }
            }

        }

        public override void SetBroadcastData(byte[] data)
        {
            /* 
             * 03 00 15 09 21 07 09 15
             *
             * Aufbau eines Datagramms: 
             * Im ersten Byte steht die Bahnnummer ( 03 )
             * Im zweiten Byte kann die Spielgruppe stehen ( 00 )
             * In jedem weiteren Byte kommen die laufenden Spiele, erst der Wert der linken Mannschaft,
             * dann der Wert der rechten Mannschaft
             * 
             */

            if (data == null)
                return;

            try
            {

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"{data.Length} -- Bahnnummer:{data[0]} -- {string.Join("-", data)}");
#endif
                int bytesToRemove = 1;                          // Ein oder Zwei Bytes, je nach dem, ob eine Spielgruppe mitgesendet wird oder nicht
                byte bahnNumber = data[0];                      //Im ersten Byte immer die Bahnnummer
                byte groupNumber = 0;                           //Default 0
                var courtGames = GetGamesOfCourt(bahnNumber);   //Alle Spiele im Turnier auf dieser Bahn

                //Wenn das Array eine gerade Länge hat, ist im zweiten Byte die Gruppennummer hinterlegt
                if (data.Length % 2 == 0)
                {
                    groupNumber = data[1];
                    bytesToRemove = 2;
                }

                //Das erste Byte (die ersten beiden) aus dem Array wird nicht mehr benötigt. Daten in ein neues Array kopieren
                byte[] newData = new byte[data.Length - bytesToRemove];
                Array.Copy(data, bytesToRemove, newData, 0, data.Length - bytesToRemove);

                int spielZähler = 1;


                //Jedes verfügbare Spiel im Datagramm durchgehen, i+2, da jedes Spiel 2 Bytes braucht. Im ersten Byte der Wert für Link, das zweite Byte für den Wert rechts
                for (int i = 0; i < newData.Length; i += 2)
                {
                    var preGame = courtGames.FirstOrDefault(g => g.GameNumberOverAll == spielZähler - 1);
                    if (preGame != null)
                    {
                        preGame.MasterTurn.PointsTeamA = preGame.NetworkTurn.PointsTeamA;
                        preGame.MasterTurn.PointsTeamB = preGame.NetworkTurn.PointsTeamB;
                    }

                    var game = courtGames.FirstOrDefault(g => g.GameNumberOverAll == spielZähler);
                    if (game == null)
                        continue;

                    game.NetworkTurn.Reset();

                    if (IsDirectionOfCourtsFromRightToLeft)
                    {
                        if (game.TeamA.SpieleAufStartSeite.Contains(game.GameNumberOverAll))
                        {
                            // TeamA befindet sich bei diesem Spiel auf dieser Bahn rechts, 
                            // das nächste Spiel ist auf einer Bahn mit höherer oder gleicher Bahnnummer (1-> 2-> 3-> 4->...)
                            game.NetworkTurn.PointsTeamA = newData[i + 1];
                            game.NetworkTurn.PointsTeamB = newData[i];
                        }
                        else
                        {
                            // TeamA befindet sich bei diesem Spiel auf der Bahn links, das nächste Spiel ist auf einer Bahn mit niedrigerer Bahnnummer (5->4->3->2->1)
                            game.NetworkTurn.PointsTeamA = newData[i];
                            game.NetworkTurn.PointsTeamB = newData[i + 1];
                        }
                    }
                    else
                    {
                        if (game.TeamA.SpieleAufStartSeite.Contains(game.GameNumberOverAll))
                        {
                            // TeamA befindet sich in diesem Spiel auf dieser Bahn links, das nächste Spiel ist auf einer Bahn mit einer höheren Bahnnummer
                            game.NetworkTurn.PointsTeamA = newData[i];
                            game.NetworkTurn.PointsTeamB = newData[i + 1];
                        }
                        else
                        {
                            // TeamA befindet sich in diesem Spiel auf dieser Bahn rechts, das nächste Spiel ist auf einer Bahn mit einer niedrigeren Bahnnummer
                            game.NetworkTurn.PointsTeamA = newData[i + 1];
                            game.NetworkTurn.PointsTeamB = newData[i];
                        }
                    }

                    spielZähler++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                RaisePropertyChanged("");
            }
        }

        #region CodeFromOldSourceCode
        //--------------
        /// <summary>
        /// Es wird ein Spielplan generiert, in dem zwei Vereine gegeneinander spielen, ohne dass ein Verein ein vereinsinternes Spiel hat
        /// </summary>
        /// <param name="AnzahlTemaS"></param>
        /// <param name="AnzahlAussetzeR"></param>
        /// <param name="Anzahlrunden"></param>
        /// <param name="AspielWechsel"></param>
        /// <returns></returns>
        //public static SPIEL[] Generiere_SonderSpielPlan_1(int AnzahlTeamS, int AnzahlAussetzeR, int AnzahlRunden, bool AnspielWechsel)
        //{
        //    List<SPIEL> Matches = new List<SPIEL>();
        //    SPIEL sMatch;

        //    // Der erste Verein hat gerade Startnummern (2,4,6,8,10) der andere Verein die ungeraden (1-3-5-7-9)
        //    // Immer ohne Aussetzer
        //    // Anzahl der Spiele ist immer Anzahl der Teams / 2
        //    // Anzahl der Bahnen ist immer Anzhal der Teams / 2

        //    int m_AnzahlBahnen = AnzahlTeamS / 2;
        //    int m_AnzahlSpiele = AnzahlTeamS / 2;

        //    int m_BahnMerker;
        //    int m_SpielMerker;

        //    for (int r = 0; r < AnzahlRunden; r++)
        //    {
        //        //Mit Spiel 1 auf Bahn 1 wird begonnen --> es folgen alle ungeraden, da immer +2 gerechnet wird
        //        m_BahnMerker = 1;
        //        m_SpielMerker = 1;

        //        for (int x = 0; x < AnzahlTeamS / 2; x++)
        //        {

        //            for (int y = 0; y < AnzahlTeamS / 2; y++)
        //            {
        //                sMatch = new SPIEL();


        //                sMatch.Runde = r + 1;

        //                if (r % 2 == 0)
        //                {
        //                    sMatch.TeamA = 1 + (y * 2);

        //                    sMatch.TeamB = sMatch.TeamA + 1 + ((x) * 2);
        //                    if (sMatch.TeamB > AnzahlTeamS) { sMatch.TeamB = sMatch.TeamB - AnzahlTeamS; }
        //                }
        //                else
        //                {
        //                    sMatch.TeamB = 1 + (y * 2);

        //                    sMatch.TeamA = sMatch.TeamB + 1 + ((x) * 2);
        //                    if (sMatch.TeamA > AnzahlTeamS) { sMatch.TeamA = sMatch.TeamA - AnzahlTeamS; }
        //                }



        //                sMatch.Bahn = m_BahnMerker + y;
        //                if (sMatch.Bahn > m_AnzahlBahnen)
        //                {
        //                    int tempBahn = sMatch.Bahn;
        //                    sMatch.Bahn = (tempBahn - m_AnzahlBahnen);
        //                }

        //                sMatch.Spiel = m_SpielMerker;
        //                if (sMatch.Spiel > m_AnzahlSpiele)
        //                {
        //                    int tempSpiel = sMatch.Spiel;
        //                    sMatch.Spiel = (tempSpiel - m_AnzahlSpiele);
        //                }


        //                if (x % 2 != 0)
        //                {
        //                    sMatch.Anspiel = sMatch.TeamA;
        //                }
        //                else
        //                {
        //                    sMatch.Anspiel = sMatch.TeamB;
        //                }


        //                Matches.Add(sMatch);

        //            }

        //            m_BahnMerker += 2;
        //            m_SpielMerker += 2;

        //            //Es wird alles auf 2 gesetzt, es folgen alle geraden, da immer +2 gerechnet wird
        //            if (m_BahnMerker > m_AnzahlBahnen) { m_BahnMerker = 2; }
        //            if (m_SpielMerker > m_AnzahlSpiele) { m_SpielMerker = 2; }

        //        }
        //    }
        //    return Matches.ToArray();
        //}
        //-----
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class Team : TBaseClass, IEquatable<Team>
    {
        #region Fields

        private string teamName;
        private int startNumber;
        private bool isVirtual;
        private string nation;
        private readonly List<Game> games;

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// If a Team is equal to another Team depends only on the StartNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Team other)
        {
            return this.StartNumber == other.StartNumber;
        }

        #endregion

        #region Standard-Properties

        /// <summary>
        /// True, wenn es sich um virtuelles Team handelt, dass nur zur Berechnung des Spielplans verwendet wird
        /// </summary>
        public bool IsVirtual
        {
            get => this.isVirtual;
            set
            {
                if (this.isVirtual == value)
                    return;

                this.isVirtual = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Startnummer
        /// </summary>
        public int StartNumber
        {
            get => startNumber;
            set
            {
                if (startNumber == value)
                    return;

                startNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Teamname
        /// </summary>
        public string TeamName
        {
            get => teamName;
            set
            {
                if (teamName == value)
                    return;

                teamName = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Info über Kreis, Bezirk, Verband oder Nation
        /// </summary>
        public string Nation
        {
            get { return nation; }
            set
            {
                if (value == nation) return;

                nation = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Liste aller Spieler
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Maximum Number of Players for a Team
        /// </summary>
        public static int MaxNumberOfPlayers { get; } = 6;

        /// <summary>
        /// Minimum Number of Players for a Team
        /// </summary>
        public static int MinNumberOfPlayer { get; } = 0;

        /// <summary>
        /// Liste aller Spiele 
        /// </summary>
        [XmlIgnore()]
        public ReadOnlyCollection<Game> Games { get; private set; }

        /// <summary>
        /// Alle Spielnummern, bei denen die Mannschaft auf der "grünen" bzw. Start-Seite steht.
        /// Das Property wird für den NetworkService und die WertungskarteTV benötigt.
        /// </summary>
        [XmlIgnore()]
        public List<int> SpieleAufStartSeite
        {
            /* 
             * Im NetworkService werden pro Bahn die Ergebnisse übertragen. Durch das Property
             * --> IsDirectionOfCourtsFromRightToLeft <-- im Tournament wird die Richtung der Bahnnummer eingestellt
             * So kann erkannt werden ob die "steigenedeMannschaft" links oder rechts auf der Bahn steht. Die Ergebnisse                 
             * im NetworkService können dann zugeordnet werden
             * Bsp: Bahn1 befindet sich rechts, die weiteren Bahnen links davon
             * Mannschaft1 befindet sich im 1. Spiel somit auf der rechten Seite des Spielfelds
             * Die nächsten Spiele bis zur letzten Bahn ganz links sind somit "steigende Spiele", die Mannschaft1 befindet
             * sich bei diesen Spielen immer rechts. 
             */
             
            get
            {
                // Ergebnisliste mit SpielNummern (GameNumberOverAll) 
                List<int> result = new List<int>();
                
                // Nach SpielNummer sortierte Liste
                var sortedGames = Games.OrderBy(g => g.GameNumberOverAll).ToList();

                for (int i = 0; i < sortedGames.Count; i++)
                {
                    if (sortedGames[i].TeamA == this && !sortedGames[i].IsPauseGame)
                        result.Add(sortedGames[i].GameNumberOverAll);
                }
                return result;
            }
        }

        [XmlIgnore()]
        public List<int> SpieleMitAnspiel
        {
            get
            {
                List<int> result = new List<int>();
                var sortedGames = Games.OrderBy(g => g.GameNumberOverAll).ToList();

                for (int i = 0; i < sortedGames.Count; i++)
                {
                    if(sortedGames[i].StartingTeam == this)
                    {
                        result.Add(sortedGames[i].GameNumberOverAll);
                    }
                }

                return result;
            }
        }


        #endregion

        #region ReadOnly Result Properties 

        public (int positiv, int negativ) SpielPunkte
        {
            get
            {
                return (Games.Sum(g => g.GetSpielPunkte(this)),
                        Games.Sum(g => g.GetSpielPunkteGegner(this)));

            }
        }

        public (int positiv, int negativ) SpielPunkte_LIVE
        {
            get
            {
                return (Games.Sum(g => g.GetSpielPunkte(this, true)),
                        Games.Sum(g => g.GetSpielPunkteGegner(this, true)));
            }
        }

        public int SpielPunkteDifferenz
        {
            get
            {
                return SpielPunkte.positiv - SpielPunkte.negativ;
            }
        }
        public int SpielPunkteDifferenz_LIVE
        {
            get
            {
                return SpielPunkte_LIVE.positiv - SpielPunkte_LIVE.negativ;
            }
        }

        public string SpielPunkteString
        {
            get
            {
                return $"{SpielPunkte.positiv}:{SpielPunkte.negativ}";
            }
        }

        public string SpielPunkteString_LIVE
        {
            get
            {
                return $"{SpielPunkte_LIVE.positiv}:{SpielPunkte_LIVE.negativ}";

            }
        }

        public (int positiv, int negativ) StockPunkte
        {
            get
            {
                return (Games.Sum(g => g.GetStockPunkte(this)),
                        Games.Sum(o => o.GetStockPunkteGegner(this)));
            }
        }

        public (int positiv, int negativ) StockPunkte_LIVE
        {
            get
            {
                return (Games.Sum(g => g.GetStockPunkte(this, true)),
                       Games.Sum(o => o.GetStockPunkteGegner(this, true)));
            }
        }

        public string StockPunkteString
        {
            get
            {
                return $"{StockPunkte.positiv}:{StockPunkte.negativ}";
            }
        }

        public string StockPunkteString_LIVE
        {
            get
            {
                return $"{StockPunkte_LIVE.positiv}:{StockPunkte_LIVE.negativ}";
            }
        }

        public double StockNote
        {
            get
            {
                if (StockPunkte.negativ == 0)
                    return StockPunkte.positiv;

                return Math.Round((double)StockPunkte.positiv / StockPunkte.negativ, 3);

            }
        }

        public double StockNote_LIVE
        {
            get
            {
                if (StockPunkte_LIVE.negativ == 0)
                    return StockPunkte_LIVE.positiv;

                return Math.Round((double)StockPunkte_LIVE.positiv / StockPunkte_LIVE.negativ, 3);
            }
        }

        public int StockPunkteDifferenz
        {
            get
            {
                return StockPunkte.positiv - StockPunkte.negativ;
            }
        }

        public int StockPunkteDifferenz_LIVE
        {
            get
            {
                return StockPunkte_LIVE.positiv - StockPunkte_LIVE.negativ;
            }
        }


        #endregion

        #region Constructor

        public Team()
        {
            this.IsVirtual = false;
            this.StartNumber = 0;
            this.games = new List<Game>();
            this.Games = games.AsReadOnly();
            this.Players = new List<Player>();
        }


        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="TeamName"></param>
        /// <param name="CountOfDefaultPlayer"></param>
        public Team(string TeamName) : this()
        {
            this.TeamName = TeamName;

        }


        #endregion

        #region Functions

        /// <summary>
        /// Deletes alle Games
        /// </summary>
        public void ClearGames()
        {
            this.games.Clear();
            RaisePropertyChanged(nameof(Games));
        }

        /// <summary>
        /// Ein Spiel anhängen
        /// </summary>
        /// <param name="game"></param>
        public void AddGame(Game game)
        {
            this.games.Add(game);
            RaisePropertyChanged(nameof(Games));
        }

        /// <summary>
        /// Alle Spiele gruppiert nach Runde
        /// </summary>
        /// <returns></returns>
        public IOrderedEnumerable<IGrouping<int, Game>> GetGamesGroupedByRound()
        {
            return from game in Games.OrderBy(r => r.RoundOfGame)
                                     .ThenBy(g => g.GameNumber)
                   group game by game.RoundOfGame into grGames
                   orderby grGames.Key
                   select grGames;
        }

        /// <summary>
        /// Einen Spieler der Mannschaft entfernen
        /// </summary>
        /// <param name="selectedPlayer"></param>
        internal void RemovePlayer(Player selectedPlayer)
        {
            this.Players.Remove(selectedPlayer);
            RaisePropertyChanged(nameof(Players));
        }

        /// <summary>
        /// Einen Spieler der Mannschaft anfügen
        /// </summary>
        internal void AddPlayer()
        {
            this.Players.Add(new Player());
            RaisePropertyChanged(nameof(Players));
        }

        /// <summary>
        /// Startnummer und Mannschaftsname
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{StartNumber}. {TeamName}";
        }

        #endregion

    }
}

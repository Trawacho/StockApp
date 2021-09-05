using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Serialization;

namespace StockApp.BaseClasses
{
    public class Game : TBaseClass, IEquatable<Game>
    {
        #region Fields

        private int roundOfGame;
        private int gameNumber;
        private int gameNumberOverall;
        private int courtNumber;
        private bool isTeamA_Starting;
        private Turn masterTurn;
        private int startNumberTeamA;
        private int startNumberTeamB;

        #endregion

        #region IEquatable Implementation

        public bool Equals(Game other)
        {
            return (this.RoundOfGame == other.RoundOfGame
                 && this.CourtNumber == other.CourtNumber
                 && this.GameNumber == other.GameNumber);
        }
        public override bool Equals(object obj)
        {
            if (obj is Game game)
            {
                return this.Equals(game);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// SpielRunde (für DoppelRunde oder mehrfachrunden)
        /// </summary>
        public int RoundOfGame
        {
            get => roundOfGame;
            set
            {
                if (roundOfGame == value)
                    return;
                roundOfGame = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Nummer vom laufenden Spiel
        /// </summary>
        public int GameNumber
        {
            get => gameNumber; set
            {
                if (gameNumber == value)
                    return;

                gameNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Nummer des Spiels über alle Runden hinweg
        /// </summary>
        public int GameNumberOverAll
        {
            get => gameNumberOverall;
            set
            {
                if (gameNumberOverall == value) return;
                gameNumberOverall = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Nummer der Spielbahn
        /// </summary>
        public int CourtNumber
        {
            get => courtNumber;
            set
            {
                if (courtNumber == value)
                    return;

                courtNumber = value;
                RaisePropertyChanged();
            }

        }

        /// <summary>
        /// Team A - 1
        /// </summary>
        [XmlIgnore()]
        public Team TeamA { get; set; }

        /// <summary>
        /// StartNumber of TeamA 
        /// </summary>
        [XmlElement(ElementName = nameof(StartNumberTeamA))]
        public int StartNumberTeamA
        {
            get
            {
                return TeamA?.StartNumber ?? startNumberTeamA;
            }
            [Obsolete("Only used for xml serialization", error: true)]
            set
            {
                if (TeamA != null)
                {
                    throw new NotSupportedException("Setting the StartNumberTeamA property is not supported");
                }

                startNumberTeamA = value;
            }
        }

        /// <summary>
        /// Team B - 2
        /// </summary>
        [XmlIgnore()]
        public Team TeamB { get; set; }

        /// <summary>
        /// StartNumber of TeamB
        /// </summary>
        [XmlElement(ElementName = nameof(StartNumberTeamB))]
        public int StartNumberTeamB
        {
            get
            {
                return TeamB?.StartNumber ?? startNumberTeamB;
            }
            [Obsolete("Only used for xml serialization", error: true)]
            set
            {
                if (TeamB != null)
                    throw new NotSupportedException("Setting the StartNumberTeamB property is not supported");

                startNumberTeamB = value;
            }
        }
        /// <summary>
        /// Das Team A hat Anspiel
        /// </summary>
        public bool IsTeamA_Starting
        {
            get => isTeamA_Starting;
            set
            {
                if (isTeamA_Starting == value)
                    return;

                isTeamA_Starting = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Das anspielende Team
        /// </summary>
        public Team StartingTeam
        {
            get
            {
                return IsTeamA_Starting ? TeamA : TeamB;
            }
        }

        /// <summary>
        /// Das NICHT anspielende Team
        /// </summary>
        public Team NotStartingTeam
        {
            get
            {
                return IsTeamA_Starting ? TeamB : TeamA;
            }
        }

        /// <summary>
        /// Wer ist der Gegner
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public Team GetOpponent(Team team)
        {
            // return team == TeamA ? TeamB : TeamA;
            return team == TeamA
                         ? TeamB
                         : team == TeamB
                                 ? TeamA
                                 : null;
        }

        /// <summary>
        /// Dieses Spiel ist ein Aussetzer, der Gegner ist ein Virtueller
        /// </summary>
        public bool IsPauseGame
        {
            get
            {
                if (TeamA.IsVirtual || TeamB.IsVirtual)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// normales Spiel, kein Aussetzer
        /// </summary>
        public bool IsNotPauseGame
        {
            get
            {
                return !IsPauseGame;
            }
        }

        /// <summary>
        /// Turn to work with StockTVs results
        /// </summary>
        public Turn NetworkTurn { get; set; }

        /// <summary>
        /// ErgebnisKehre. Wenn hier Werte enthalten sind, dann wird die Liste der Kehren nicht berücksichtigt
        /// </summary>
        public Turn MasterTurn
        {
            get
            {
                return masterTurn;
            }
            set
            {
                if (masterTurn == value) return;
                masterTurn = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Summe der Stockpunkte von TeamB in diesem Spiel
        /// </summary>
        public int StockPointsTeamB
        {
            get
            {
                return MasterTurn.PointsTeamB;
            }
        }

        public int StockPointsTeamB_LIVE
        {
            get
            {
                return NetworkTurn?.PointsTeamB ?? 0;
            }
        }

        /// <summary>
        /// Summe der Stockpunkte von TeamA in diesem Spiel
        /// </summary>
        public int StockPointsTeamA
        {
            get
            {
                return MasterTurn.PointsTeamA;
            }
        }

        public int StockPointsTeamA_LIVE
        {
            get
            {
                return NetworkTurn?.PointsTeamA ?? 0;
            }
        }


        /// <summary>
        /// Spielpunkte von TeamB in diesem Spiel
        /// </summary>
        public int SpielPunkteTeamB
        {
            get
            {
                if (StockPointsTeamA == 0 && StockPointsTeamB == 0)
                    return 0;

                if (StockPointsTeamA > StockPointsTeamB)
                    return 0;

                if (StockPointsTeamB > StockPointsTeamA)
                    return 2;

                return 1;
            }
        }

        public int SpielPunkteTeamB_LIVE
        {
            get
            {
                if (StockPointsTeamA_LIVE == 0 && StockPointsTeamB_LIVE == 0)
                    return 0;

                if (StockPointsTeamA_LIVE > StockPointsTeamB_LIVE)
                    return 0;

                if (StockPointsTeamB_LIVE > StockPointsTeamA_LIVE)
                    return 2;

                return 1;
            }
        }

        /// <summary>
        /// Spielpunkte von TeamA in diesem Spiel
        /// </summary>
        public int SpielPunkteTeamA
        {
            get
            {
                if (StockPointsTeamA == 0 && StockPointsTeamB == 0)
                    return 0;

                if (StockPointsTeamA > StockPointsTeamB)
                    return 2;

                if (StockPointsTeamB > StockPointsTeamA)
                    return 0;

                return 1;
            }
        }

        public int SpielPunkteTeamA_LIVE
        {
            get
            {
                if (StockPointsTeamA_LIVE == 0 && StockPointsTeamB_LIVE == 0)
                    return 0;

                if (StockPointsTeamA_LIVE > StockPointsTeamB_LIVE)
                    return 2;

                if (StockPointsTeamB_LIVE > StockPointsTeamA_LIVE)
                    return 0;

                return 1;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Game()
        {
            RoundOfGame = 1;
            // this.Turns = new ConcurrentStack<Turn>();
            this.NetworkTurn = new Turn();
            this.MasterTurn = new Turn();   //Default-Kehere für manuelle Eingabe
        }



        #endregion

        #region Public Functions

        public override string ToString()
        {
            return $"R#:{RoundOfGame} C#:{CourtNumber} G#:{GameNumber}({GameNumberOverAll}) -- {TeamA.StartNumber} : {TeamB.StartNumber}    T1A:{IsTeamA_Starting}     P:{IsPauseGame} ";
        }

        /// <summary>
        /// Returns RoundOfGame and GameNumber as a string
        /// </summary>
        public string GameName
        {
            get
            {
                return $"Runde: {RoundOfGame} | Spiel: {GameNumber}";
            }
        }

        public int GetStockPunkte(Team team, bool live = false)
        {
            if (team == TeamA)
                return live ? StockPointsTeamA_LIVE : StockPointsTeamA;
            if (team == TeamB)
                return live ? StockPointsTeamB_LIVE : StockPointsTeamB;
            return 0;
        }

        public int GetStockPunkteGegner(Team team, bool live = false)
        {
            if (team == TeamA)
                return live ? StockPointsTeamB_LIVE : StockPointsTeamB;
            if (team == TeamB)
                return live ? StockPointsTeamA_LIVE : StockPointsTeamA;
            return 0;
        }

        public int GetSpielPunkte(Team team, bool live = false)
        {
            if (team == TeamA)
                return live ? SpielPunkteTeamA_LIVE : SpielPunkteTeamA;
            if (team == TeamB)
                return live ? SpielPunkteTeamB_LIVE : SpielPunkteTeamB;
            return 0;
        }

        public int GetSpielPunkteGegner(Team team, bool live = false)
        {
            if (team == TeamA)
                return live ? SpielPunkteTeamB_LIVE : SpielPunkteTeamB;
            if (team == TeamB)
                return live ? SpielPunkteTeamA_LIVE : SpielPunkteTeamA;
            return 0;
        }


        #endregion
    }
}

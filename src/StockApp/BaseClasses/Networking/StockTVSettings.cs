using System;
using System.Text;

namespace StockApp.BaseClasses
{
    public class StockTVSettings : TBaseClass, IEquatable<StockTVSettings>
    {
        private int bahn;
        private int pointsPerTurn;
        private int turnsPerGame;
        private int spielgruppe;
        private NextBahnModis nextBahnModus;
        private GameModis gameModus;
        private ColorModis colorModus;

        public static StockTVSettings GetDefault(GameModis modus)
        {
            var s = new StockTVSettings()
            {
                Bahn = -1,
                NextBahnModus = NextBahnModis.Left,
                ColorModus = ColorModis.Normal,
                GameModus = modus
            };
            switch (modus)
            {
                case GameModis.Training:
                    s.PointsPerTurn = 30;
                    s.TurnsPerGame = 30;
                    break;
                case GameModis.Turnier:
                case GameModis.BestOf:
                    s.PointsPerTurn = 15;
                    s.TurnsPerGame = 6;
                    break;
                case GameModis.Ziel:
                    break;
                default:
                    break;
            }

            return s;
        }

        private StockTVSettings()
        {

        }

        public StockTVSettings(byte[] array) : this()
        {
            SetValues(array);
        }

        public StockTVSettings(string valueString) : this()
        {
            SetValues(valueString);
        }

        public int Bahn { get => bahn; set => SetProperty(ref bahn, value); }
        public int Spielgruppe { get => spielgruppe; set => SetProperty(ref spielgruppe, value); }
        public int PointsPerTurn { get => pointsPerTurn; set => SetProperty(ref pointsPerTurn, value); }
        public int TurnsPerGame { get => turnsPerGame; set => SetProperty(ref turnsPerGame, value); }
        public NextBahnModis NextBahnModus { get => nextBahnModus; set => SetProperty(ref nextBahnModus, value); }
        public GameModis GameModus
        {
            get => gameModus; 
            set
            {
                SetProperty(ref gameModus, value);
                switch (value)
                {
                    case GameModis.Training:
                        PointsPerTurn = 30;
                        TurnsPerGame = 30;
                        break;
                    case GameModis.Turnier:
                    case GameModis.BestOf:
                        PointsPerTurn = 15;
                        TurnsPerGame = 6;
                        break;
                    case GameModis.Ziel:
                    default:
                        break;
                }
            }
        }
        public ColorModis ColorModus { get => colorModus; set => SetProperty(ref colorModus, value); }

        public bool Equals(StockTVSettings other)
        {
            return Bahn == other.Bahn
                && PointsPerTurn == other.PointsPerTurn
                && TurnsPerGame == other.TurnsPerGame
                && NextBahnModus == other.NextBahnModus
                && GameModus == other.GameModus
                && ColorModus == other.ColorModus;
        }

        /// <summary>
        /// Copies Settings except of Bahn
        /// </summary>
        /// <param name="s"></param>
        internal void CopyFrom(StockTVSettings s)
        {
            this.PointsPerTurn = s.PointsPerTurn;
            this.TurnsPerGame = s.TurnsPerGame;
            this.Spielgruppe = s.Spielgruppe;
            this.NextBahnModus = s.NextBahnModus;
            this.GameModus = s.GameModus;
            this.ColorModus = s.ColorModus;
        }

        /// <summary>
        /// Splits the parameter on ";" and set every property
        /// </summary>
        /// <param name="valueString"></param>
        private void SetValues(string valueString)
        {
            var parts = valueString.TrimEnd(';').Split(';');

            foreach (var part in parts)
            {
                var x = part.Split('=');
                var topic = x[0];
                var value = x[1];
                switch (topic)
                {
                    case nameof(Bahn):
                        Bahn = int.Parse(value);
                        break;
                    case nameof(Spielgruppe):
                        Spielgruppe = int.Parse(value);
                        break;
                    case nameof(PointsPerTurn):
                        PointsPerTurn = int.Parse(value);
                        break;
                    case nameof(TurnsPerGame):
                        TurnsPerGame = int.Parse(value);
                        break;
                    case nameof(NextBahnModus):
                        NextBahnModus = (NextBahnModis)Enum.Parse(typeof(NextBahnModis), value);
                        break;
                    case nameof(GameModus):
                        GameModus = (GameModis)Enum.Parse(typeof(GameModis), value);
                        break;
                    case nameof(ColorModus):
                        ColorModus = (ColorModis)Enum.Parse(typeof(ColorModis), value);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetValues(byte[] array)
        {
            SetValues(Encoding.UTF8.GetString(array));
        }

        public override string ToString()
        {
            return $"Bahn:{Bahn} | Spielgruppe:{Spielgruppe} | GameModus:{GameModus} | ColorModus:{ColorModus} | PointsPerTurn:{PointsPerTurn} | TurnsPerGame:{TurnsPerGame} | NextBahn:{NextBahnModus}  ";
        }
    }
}

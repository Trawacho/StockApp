namespace StockApp.BaseClasses
{


    /// <summary>
    /// Eine Kehre im Spiel
    /// </summary>
    public class Turn : TBaseClass
    {
        #region Fields

        private int pointsTeamA;
        private int pointsTeamB;

        #endregion

        #region Properties

        /// <summary>
        /// Stockpunkte von Team A in dieser Kehre
        /// </summary>
        public int PointsTeamA
        {
            get => pointsTeamA;
            set
            {
                if (pointsTeamA == value)
                    return;

                pointsTeamA = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Stockpunkte von Team B in dieser Kehre
        /// </summary>
        public int PointsTeamB
        {
            get => pointsTeamB;
            set
            {
                if (pointsTeamB == value)
                    return;

                pointsTeamB = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        #region Constructor

        public Turn()
        {
            PointsTeamA = 0;
            PointsTeamB = 0;
        }

        #endregion

        #region Function

        internal void Reset()
        {
            PointsTeamA = 0;
            PointsTeamB = 0;
        }


        #endregion
    }
}

using System;
using System.Linq;

namespace StockApp.BaseClasses
{
    public class NetworkTelegram: IEquatable<NetworkTelegram>
    {
        private readonly byte[] telegram;

        public NetworkTelegram(byte[] telegram)
        {
            this.telegram = telegram;
        }

        public byte[] Values
        {
            get
            {
                byte[] values = new byte[telegram.Length - 10];
                Array.Copy(telegram, 10, values, 0, telegram.Length - 10);
                return values;
            }
        }

        public byte BahnNummer => telegram[0];

        public byte SpielGruppe => telegram[1];

        /// <summary>
        /// Wert kommt aus StockTV GameSettings.GameModis
        /// <br>0 = Training</br>
        /// <br>1 = BestOf</br>
        /// <br>2 = Turnier</br>
        /// <br>100 = Ziel</br>
        /// </summary>
        public byte StockTVModus => telegram[2];

        /// <summary>
        /// <para>Wert aus StockTV ColorScheme.NextBahnModis</para>
        /// <br>0 = Links</br>
        /// <br>1 = Rechts</br>
        /// </summary>
        public byte Spielrichtung => telegram[3];

        public bool Equals(NetworkTelegram other)
        {
            if (other == null) return false;
            return telegram.SequenceEqual(other.telegram );
        }

        internal NetworkTelegram Copy()
        {
            byte[] newTelegram = new byte[telegram.Length];
            Array.Copy(telegram, 0, newTelegram, 0, telegram.Length);
            return new NetworkTelegram( newTelegram);
        }
    }

}

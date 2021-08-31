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

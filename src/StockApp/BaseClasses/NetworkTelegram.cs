using System;

namespace StockApp.BaseClasses
{
    public class NetworkTelegram
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

    }

}

using StockApp.BaseClasses.Zielschiessen;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace StockApp.BaseClasses
{
    public delegate void NetworkServiceEnventHandler(object sender, NetworkServiceDataReceivedEventArgs e);
    public sealed class NetworkService
    {
        private static readonly Lazy<NetworkService> lazy =
        new Lazy<NetworkService>(() => new NetworkService());

        public static NetworkService Instance { get { return lazy.Value; } }

        private UdpClient udpClient;
        private UdpState state;
        private readonly Tournament tournament;
        private readonly Action _CallBackAfterUpdateAction;

        public event EventHandler StartStopStateChanged;
        void OnStartStopStateChanged(NetworkServiceEventArgs e)
        {
            StartStopStateChanged?.Invoke(this, new NetworkServiceEventArgs(isRunning));
        }

        public event NetworkServiceEventHandler DataReceived;
        void RaiseDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, new NetworkServiceDataReceivedEventArgs(data));
        }

       


        public NetworkService()
        {
                
        }


        public void Start()
        {
            if (udpClient == null)
            {
                udpClient = new UdpClient();
                udpClient.Client.ReceiveTimeout = 500;
                udpClient.EnableBroadcast = true;
                udpClient.Client.Blocking = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Settings.Instanze.BroadcastPort));

            }
            if (state == null)
            {
                state = new UdpState()
                {
                    udpClient = udpClient,
                    ipEndPoint = new IPEndPoint(0,0),
                };
            }

            ReceiveBroadcast();
            OnStartStopStateChanged(true);
        }

        public void Stop()
        {
            udpClient.Close();
            udpClient.Dispose();
            udpClient = null;
            state.udpClient = null;
            state = null;

            OnStartStopStateChanged(false);
        }

        public bool IsRunning()
        {
            return (udpClient != null);
        }

        private void ReceiveBroadcast()
        {
            state.result = state.udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((UdpState)ar.AsyncState).udpClient;
                IPEndPoint e = ((UdpState)ar.AsyncState).ipEndPoint;
                IAsyncResult r = ((UdpState)ar.AsyncState).result;

                byte[] receiveBytes = u?.EndReceive(ar, ref e);
                if (receiveBytes?.Length > 1)
                {
                    RaiseDataReceived(DeCompress(receiveBytes));
                }

                r = u?.BeginReceive(new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ReceiveCallback: {e.Message}");
            }
        }

        byte[] DeCompress(byte[] data)
        {
            if (data == null)
                return null;

            using MemoryStream input = new MemoryStream(data);
            using MemoryStream output = new MemoryStream();
            using (var datastream = new DeflateStream(input, CompressionMode.Decompress))
            {
                datastream.CopyTo(output);
            }

            return output.ToArray();
        }

        void DeSerializeZielBewerb(byte[] data)
        {
            /*
             * 03 04 08 00 06 10 02 05 10 02 00 10 
             * 
             * Aufbau: 
             * Im ersten Byte steht die Bahnnummer ( 03 )
             * In jedem weiteren Byte kommen die laufenden Versuche (max 24) 
             * Somit ist ein Datagramm max 25 Bytes lang
             * 
             */
            if (data == null)
                return;

            try
            {

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"{data.Length} -- Bahnnummer:{data[0]} -- {string.Join("-", data)}");
#endif

                if ((bewerb as Zielbewerb).Teilnehmerliste.FirstOrDefault(t => t.AktuelleBahn == data[0]) is Teilnehmer spieler)
                {
                    if (spieler.Onlinewertung.VersucheAllEntered() && data.Length == 1)
                    {
                        spieler.DeleteAktuellBahn();
                    }
                    else
                    {
                        spieler.Onlinewertung.Reset();

                        for (int i = 1; i < data.Length; i++)
                        {
                            spieler?.SetVersuch(i, data[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeSerializeZielBewerb: {ex.Message}");
            }
            finally
            {
                _CallBackAfterUpdateAction?.Invoke();
            }

        }
        void DeSerializeTeamBewerb(byte[] data)
        {
            /* 
             * 03 15 09 21 07 09 15
             *
             * Aufbau eines Datagramms: 
             * Im ersten Byte steht die Bahnnummer ( 03 )
             * In jedem weiteren Byte kommen die laufenden Spiele, erst der Wert der linken Mannschaft,
             * dann der Wert der rechten Mannschaft
             * 
             */

            if (data == null)
                return;

            //Es muss immer eine ungerade Anzahl an Daten vorhanden sein
            if (data.Length % 2 == 0)
                return;

            try
            {

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"{data.Length} -- Bahnnummer:{data[0]} -- {string.Join("-", data)}");
#endif

                byte bahnNumber = data[0];
                var courtGames = (bewerb as TeamBewerb).GetGamesOfCourt(bahnNumber);

                //Das erste Byte aus dem Array wird nicht mehr benötigt. Daten in ein neues Array kopieren
                byte[] newData = new byte[data.Length - 1];
                Array.Copy(data, 1, newData, 0, data.Length - 1);

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

                    if ((bewerb as TeamBewerb).IsDirectionOfCourtsFromRightToLeft)
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
                System.Diagnostics.Debug.WriteLine($"DeSerializeTeamBewerb: {ex.Message}");
            }
            finally
            {
                _CallBackAfterUpdateAction?.Invoke();
            }
        }



        private class UdpState
        {
            public UdpClient udpClient;
            public IPEndPoint ipEndPoint;
            public IAsyncResult result;
        }
    }
}

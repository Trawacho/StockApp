using System.Linq;

namespace StockApp.BaseClasses.Zielschiessen
{
    internal static class ZielBewerbExtension
    {
        

        public static Turnier GetSampleZielBewerbTurnier()
        {
            Turnier turnier = new Turnier
            {
                OrgaDaten = new OrgaDaten()
            };
            turnier.SetBewerb(Wettbewerbsart.Ziel);

            var bewerb = turnier.Wettbewerb as Zielbewerb;
            bewerb.AddNewTeilnehmer();
            bewerb.AddNewTeilnehmer();
            bewerb.AddNewTeilnehmer();
            bewerb.AddNewTeilnehmer();
            bewerb.AddNewTeilnehmer();

            var teilnehmer = bewerb.Teilnehmerliste.First(t => t.Startnummer == 1);
            teilnehmer.FirstName = "Hans";
            teilnehmer.LastName = "Dampf";
            teilnehmer.Vereinsname = "ESF Hankofen";
            teilnehmer.LicenseNumber = "02/85859";
            teilnehmer.AddNewWertung();
            teilnehmer.AddNewWertung();
            teilnehmer.AddNewWertung();
            teilnehmer.AddNewWertung();

            teilnehmer.SetAktuelleBahn(1, 1);


            return turnier;
        }
       
    }
}

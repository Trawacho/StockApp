using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.BaseClasses
{
    public class OrgaDaten : TBaseClass
    {
        /// <summary>
        /// Veranstaltungsort
        /// </summary>
        public string Venue { get; set; }

        /// <summary>
        /// Organisator / Veranstalter
        /// </summary>
        public string Organizer { get; set; }

        /// <summary>
        /// Datum / Zeit des Turniers
        /// </summary>
        public DateTime DateOfTournament { get; set; }

        /// <summary>
        /// Durchführer
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Turniername
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// Startgebühr pro Mannschaft
        /// </summary>
        public Startgebuehr EntryFee { get; set; }
        public Schiedsrichter Referee { get; set; }
        public Wettbewerbsleiter CompetitionManager { get; set; }
        public Rechenbuero ComputingOfficer { get; set; }


        public OrgaDaten()
        {
            this.DateOfTournament = DateTime.Now;
            this.EntryFee = new Startgebuehr();
            this.Referee = new Schiedsrichter();
            this.CompetitionManager = new Wettbewerbsleiter();
            this.ComputingOfficer = new Rechenbuero();
        }

    }
}

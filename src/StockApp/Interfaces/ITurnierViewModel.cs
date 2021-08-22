using StockApp.BaseClasses;
using System;
using System.Collections.Generic;

namespace StockApp.Interfaces
{
    /// <summary>
    /// Stellt die Organisationsdaten und die Basisdaten eines Turniers da
    /// </summary>
    public interface ITurnierViewModel
    {
        /// <summary>
        /// Veranstalter
        /// </summary>
        string TurnierOrt { get; set; }
        string Veranstalter { get; set; }
        string Durchfuehrer { get; set; }
        string TurnierName { get; set; }
        DateTime TurnierDatum { get; set; }
        Startgebuehr Startgebuehr { get; set; }
        int SpielGruppe { get; set; }
        Schiedsrichter Schiedsrichter { get; set; }
        Wettbewerbsleiter Wettbewerbsleiter { get; set; }
        Rechenbuero Rechenbuero { get; set; }
        Wettbewerbsart Wettbewerbsart { get; set; }
        List<Wettbewerbsart> Wettbewerbsarten { get; }
    }
}

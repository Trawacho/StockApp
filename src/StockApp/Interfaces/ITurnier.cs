using StockApp.BaseClasses;

namespace StockApp.Interfaces
{
    internal interface ITurnier
    {
        int SpielGruppe { get; set; }
        string SpielGruppeString();

        OrgaDaten OrgaDaten { get; set; }

        public IBaseBewerb Wettbewerb { get; set; }


    }
}


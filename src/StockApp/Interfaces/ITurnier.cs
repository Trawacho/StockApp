using StockApp.BaseClasses;

namespace StockApp.Interfaces
{
    internal interface ITurnier
    {
        

        OrgaDaten OrgaDaten { get; set; }

        public IBaseBewerb Wettbewerb { get;  }


    }
}


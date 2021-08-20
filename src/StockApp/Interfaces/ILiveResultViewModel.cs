using StockApp.BaseClasses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.Interfaces
{
    public interface ILiveResultViewModel
    {
        bool IsLive { get; set; }
        bool ShowDifferenz { get; set; }
        bool ShowStockPunkte { get; set; }
        bool IsListenerOnline { get; set; }
        string WindowTitle { get; }
        ObservableCollection<(int Platzierung, Team Team, bool isLive)> Ergebnisliste { get; }
        ICommand RefreshCommand { get; }
        ICommand CloseCommand { get; }
    }
}

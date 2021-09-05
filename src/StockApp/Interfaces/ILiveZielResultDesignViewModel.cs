using StockApp.BaseClasses.Zielschiessen;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.Interfaces
{
    public interface ILiveZielResultDesignViewModel
    {
        public ObservableCollection<(int Platzierung, Teilnehmer Spieler, bool isLive)> Ergebnisliste { get; }
        public string WindowTitle { get; }
        public ICommand CloseCommand { get; }
        public ICommand RefreshCommand { get; }
        public bool IsLive { get; set; }
    }
}

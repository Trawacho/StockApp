using StockApp.BaseClasses.Zielschiessen;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.Interfaces
{
    public interface IZielSpielerViewModel
    {
        public Teilnehmer SelectedPlayer { get; set; }
        public Wertung SelectedWertung { get; set; }

        public int SelectedZielBahn { get; set; }

        ObservableCollection<Teilnehmer> Players { get; }
        Zielbewerb Bewerb { get; }

        ICommand AddPlayerCommand { get; }
        ICommand RemovePlayerCommand { get; }
        ICommand AddWertungCommand { get; }
        ICommand RemoveWertungCommand { get; }
        ICommand SetWertungOnlineCommand { get; }
    }
}

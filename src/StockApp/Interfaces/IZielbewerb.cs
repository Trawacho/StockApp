using StockApp.BaseClasses.Zielschiessen;
using System.Linq;

namespace StockApp.Interfaces
{
    public interface IZielbewerb
    {
        IOrderedEnumerable<Teilnehmer> Teilnehmerliste { get; }
        IOrderedEnumerable<int> Bahnen { get; }
        IOrderedEnumerable<int> FreieBahnen { get; }
        void AddNewTeilnehmer();
        bool CanAddTeilnehmer();
        void RemoveTeilnehmer(Teilnehmer teilnehmer);
        bool CanRemoveTeilnehmer();
        void MoveTeilnehmer(int oldIndex, int newIndex);
    }
}

using StockApp.BaseClasses;
using StockApp.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace StockApp.Output.WertungsKarteBase
{
    public abstract class WertungskarteBase
    {
        internal FixedDocument document;
        public WertungskarteBase()
        {

        }

        public FixedDocument GetDocument(Size pageSize, TeamBewerb tournament, bool printTeamName, bool concatRounds)
        {
            document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            var teamPanels = new List<StackPanel>();

            foreach (var team in tournament.Teams.Where(v => !v.IsVirtual)
                                                 .OrderBy(t => t.StartNumber)) //Für jedes Team eine Spiegel-Karte
            {
                if (concatRounds)
                {
                    teamPanels.Add(GetTeamPanel(team, printTeamName, tournament.Is8TurnsGame));
                }
                else
                {
                    teamPanels.AddRange(GetTeamPanels(team, printTeamName, tournament.Is8TurnsGame));
                }
            }

            var pagePanel = new StackPanel();

            foreach (var teamPanel in teamPanels)
            {

                pagePanel.Children.Add(teamPanel);
                pagePanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                pagePanel.Arrange(new Rect(0, 0, pagePanel.DesiredSize.Width, pagePanel.DesiredSize.Height));

                if (pagePanel.ActualHeight + teamPanel.ActualHeight > document.DocumentPaginator.PageSize.Height)
                {
                    SetPagePanelToDocument(pagePanel);
                    pagePanel = new StackPanel();
                }
            }
            if (pagePanel.Children.Count > 0)
            {
                SetPagePanelToDocument(pagePanel);
            }

            return document;
        }

        internal abstract  List<StackPanel> GetTeamPanels(Team team, bool printTeamName, bool kehren8);
        internal abstract StackPanel GetTeamPanel(Team team, bool printTeamName, bool kehren8);

        internal  FixedPage GetNewPage(Size pageSize)
        {
            FixedPage page = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height
            };
            return page;
        }

        internal void SetPagePanelToDocument(StackPanel panel)
        {
            var newPage = GetNewPage(document.DocumentPaginator.PageSize);

            //Wenn die aktuelle Höhe + die neue Höhe > seiten-Höhe
            FixedPage.SetTop(panel, PixelConverter.CmToPx(1));
            FixedPage.SetLeft(panel, PixelConverter.CmToPx(0.7));
            newPage.Children.Add(panel);

            PageContent content = new PageContent();
            ((IAddChild)content).AddChild(newPage);
            document.Pages.Add(content);
        }

    }
}

using StockApp.BaseClasses;
using StockApp.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Linq;
using System.Collections.Generic;

namespace StockApp.Output.Bahnblock
{
    internal class BahnBlock
    {
        FixedDocument document;
        public BahnBlock()
        {

        }

        public FixedDocument GetDocument(Size pageSize, TeamBewerb tournament)
        {
            document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            var allGames = tournament.GetAllGames().Where(g => g.IsNotPauseGame && g.CourtNumber > 0);

            var bahnblöcke = new List<StackPanel>();

            foreach (var game in allGames.OrderBy(b => b.CourtNumber).ThenBy(r => r.RoundOfGame).ThenBy(g => g.GameNumber))
            {
                bahnblöcke.Add(GetNewBahnblock(game, tournament.Is8TurnsGame));
            }

            var pagePanel = new StackPanel();

            foreach (var bahnblock in bahnblöcke)
            {
                pagePanel.Children.Add(bahnblock);
                pagePanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                pagePanel.Arrange(new Rect(0, 0, pagePanel.DesiredSize.Width, pagePanel.DesiredSize.Height));

                if (pagePanel.ActualHeight + bahnblock.ActualHeight > document.DocumentPaginator.PageSize.Height)
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

        private StackPanel GetNewBahnblock(Game game, bool is8KehrenSpiel)
        {
            StackPanel panel = new StackPanel();
            panel.Children.Add(Tools.CutterLineTop());
            
            if (is8KehrenSpiel)
                panel.Children.Add(new ucBahnBlock8(game));
            else
                panel.Children.Add(new ucBahnBlock(game));

            panel.Children.Add(
                new TextBlock()
                {
                    Text = "created by StockApp",
                    FontSize = 7,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left
                });

            panel.Children.Add(Tools.CutterLine());

            return panel;
        }

        private FixedPage GetNewPage(Size pageSize)
        {
            FixedPage page = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height
            };
            return page;
        }

        private void SetPagePanelToDocument(StackPanel panel)
        {
            var newPage = GetNewPage(document.DocumentPaginator.PageSize);

            //Wenn die aktuelle Höhe + die neue Höhe > seiten-Höhe
            FixedPage.SetTop(panel, PixelConverter.CmToPx(1));
            FixedPage.SetLeft(panel, PixelConverter.CmToPx(2.0));
            newPage.Children.Add(panel);

            PageContent content = new PageContent();
            ((IAddChild)content).AddChild(newPage);
            document.Pages.Add(content);
        }


    }
}

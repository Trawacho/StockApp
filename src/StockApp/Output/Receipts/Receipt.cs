using StockApp.BaseClasses;
using StockApp.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace StockApp.Output.Receipts
{
    public class Receipt
    {
        FixedDocument document;
        private readonly Turnier turnier;
        public Receipt(Turnier turnier)
        {
            this.turnier = turnier;
        }

        public FixedDocument CreateReceipts(Size pageSize)
        {
            document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;


            var receipts = new List<StackPanel>();

            foreach (var team in (turnier.Wettbewerb as TeamBewerb).Teams.Where(t => !t.IsVirtual))
            {
                var receiptStackPanel = new StackPanel();
                receiptStackPanel.Children.Add(Tools.CutterLineTop());

                var receipt = new ucReceipt();
                receipt.labelAn.Content = turnier.OrgaDaten.Organizer;
                receipt.labelVon.Content = team.TeamName;
                receipt.labelEUR.Content = turnier.OrgaDaten.EntryFee.Value.ToString("C");
                receipt.labelVerbal.Content = $"-- {turnier.OrgaDaten.EntryFee.Verbal} --";
                receipt.labelZweck.Content = turnier.OrgaDaten.TournamentName;
                receipt.labelOrtDatum.Content = turnier.OrgaDaten.Venue + ", " + turnier.OrgaDaten.DateOfTournament.ToString("dd.MM.yyyy");
                receiptStackPanel.Children.Add(receipt);

                receiptStackPanel.Children.Add(Tools.CutterLine());

                receipts.Add(receiptStackPanel);
            }


            var pagePanel = new StackPanel();

            foreach (var receipt in receipts)
            {

                pagePanel.Children.Add(receipt);
                pagePanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                pagePanel.Arrange(new Rect(0, 0, pagePanel.DesiredSize.Width, pagePanel.DesiredSize.Height));

                if (pagePanel.ActualHeight + receipt.ActualHeight > document.DocumentPaginator.PageSize.Height)
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

        private void SetPagePanelToDocument(StackPanel panel)
        {
            var newPage = Helpers.GetNewPage(document.DocumentPaginator.PageSize);
            newPage.HorizontalAlignment = HorizontalAlignment.Center;
            newPage.VerticalAlignment = VerticalAlignment.Center;

            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;

            FixedPage.SetTop(panel, PixelConverter.CmToPx(1));
            FixedPage.SetLeft(panel, PixelConverter.CmToPx(2.0));
            newPage.Children.Add(panel);

            PageContent content = new PageContent();
            ((IAddChild)content).AddChild(newPage);
            document.Pages.Add(content);
        }


    }
}

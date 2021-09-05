using StockApp.BaseClasses;
using StockApp.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace StockApp.Output.Results
{
    public class Result
    {
        private FixedDocument document;
        private readonly TeamBewerb teamBewerb;
        private readonly Turnier turnier;

        public Result(Turnier turnier)
        {
            this.turnier = turnier;
            this.teamBewerb = turnier.Wettbewerb as TeamBewerb;
        }


        public FixedDocument CreateResult(Size pageSize)
        {
            document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            //Ergebnisliste - Überschriften
            var ergListe = new ucResult();
            ergListe._ArtDesWettbewerbs.Content = turnier.OrgaDaten.TournamentName;
            ergListe._Austragungsort.Content = turnier.OrgaDaten.Venue;
            ergListe._Durchführer.Content = turnier.OrgaDaten.Operator;
            ergListe._Veranstalter.Content = turnier.OrgaDaten.Organizer;
            ergListe._Datum.Content = turnier.OrgaDaten.DateOfTournament.ToString("dddd, dd.MM.yyyy");


            var rankedTeams = teamBewerb.GetTeamsRanked();
            int r = 1;

            foreach (var team in rankedTeams)
            {
                var spT = new ucStackPanelTeam(r, team, (r <= teamBewerb.NumberOfTeamsWithNamedPlayerOnResult));
                r++;
                ergListe._spTeams.Children.Add(spT);
            }

            //Eintragen der Offiziellen
            if (string.IsNullOrWhiteSpace(turnier.OrgaDaten.Referee.Name))
            {
                ergListe._stackPanelSchiedsrichter.Visibility = Visibility.Collapsed;
            }
            ergListe._textblockSchiedsrichterName.Text = turnier.OrgaDaten.Referee.Name;
            ergListe._textblockSchiedsrichterClub.Text = turnier.OrgaDaten.Referee.ClubName;

            if (string.IsNullOrWhiteSpace(turnier.OrgaDaten.CompetitionManager.Name))
            {
                ergListe._stackPanelWettbewerbsleiter.Visibility = Visibility.Collapsed;
            }
            ergListe._textblockWettbewerbsleiterName.Text = turnier.OrgaDaten.CompetitionManager.Name;
            ergListe._textblockWettbewerbsleiterClub.Text = turnier.OrgaDaten.CompetitionManager.ClubName;

            if (string.IsNullOrWhiteSpace(turnier.OrgaDaten.ComputingOfficer.Name))
            {
                ergListe._stackPanelRechenbüro.Visibility = Visibility.Collapsed;
            }
            ergListe._textblockRechenbüroName.Text = turnier.OrgaDaten.ComputingOfficer.Name;
            ergListe._textblockRechenbüroClub.Text = turnier.OrgaDaten.ComputingOfficer.ClubName;

            SetPagePanelToDocument(ergListe);
            return document;


        }


        private void SetPagePanelToDocument(Control control)
        {
            var newPage = Helpers.GetNewPage(document.DocumentPaginator.PageSize);
            newPage.HorizontalAlignment = HorizontalAlignment.Center;
            newPage.VerticalAlignment = VerticalAlignment.Center;

            //panel.HorizontalAlignment = HorizontalAlignment.Center;
            //panel.VerticalAlignment = VerticalAlignment.Center;

            FixedPage.SetTop(control, PixelConverter.CmToPx(1));
            FixedPage.SetLeft(control, PixelConverter.CmToPx(0.7));
            newPage.Children.Add(control);

            PageContent content = new PageContent();
            ((IAddChild)content).AddChild(newPage);
            document.Pages.Add(content);
        }




    }
}

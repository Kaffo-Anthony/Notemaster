using Microsoft.Maui.Controls.Shapes;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class FicheDetailPage : ContentPage
{
    public FicheDetailPage() => InitializeComponent();

    protected override void OnAppearing()
{
    base.OnAppearing();
    var fiche = FicheStore.Selected;
    if (fiche == null) return;

    TitleLabel.Text = fiche.Title;
    SummaryLabel.Text = fiche.Summary;

    var keywordsCountLabel = this.FindByName<Label>("KeywordsCountLabel");
    if (keywordsCountLabel != null)
    {
        keywordsCountLabel.Text = fiche.Keywords.Count <= 1
            ? $"{fiche.Keywords.Count} mot-clé"
            : $"{fiche.Keywords.Count} mots-clés";
    }

    KeywordsLayout.Children.Clear();
    foreach (var k in fiche.Keywords)
    {
        var (bg, fg) = k.Importance switch
        {
            "Élevée"  => ("#FEE2E2", "#B91C1C"),
            "Moyenne" => ("#FFEDD5", "#C2410C"),
            _         => ("#FEF3C7", "#92400E")
        };

        var name = new Label
        {
            Text = k.Name, FontFamily = "OpenSansSemibold", FontSize = 16,
            TextColor = Color.FromArgb("#14124A"), VerticalOptions = LayoutOptions.Center
        };
        var pill = new Border
        {
            BackgroundColor = Color.FromArgb(bg), StrokeThickness = 0, Padding = new Thickness(10, 4),
            StrokeShape = new RoundRectangle { CornerRadius = 10 }, VerticalOptions = LayoutOptions.Center,
            Content = new Label
            {
                Text = $"Importance : {k.Importance}", FontSize = 12,
                FontFamily = "OpenSansSemibold", TextColor = Color.FromArgb(fg)
            }
        };
        var header = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 10
        };
        header.Add(name, 0, 0);
        header.Add(pill, 1, 0);

        var definition = new Label
        {
            Text = k.Definition, FontSize = 14, FontFamily = "OpenSansRegular",
            TextColor = Color.FromArgb("#1F2340"), IsVisible = false
        };
        var card = new Border
        {
            Padding = 14, BackgroundColor = Colors.White, Stroke = Color.FromArgb("#E3E6F3"), StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Content = new VerticalStackLayout { Spacing = 8, Children = { header, definition } }
        };
        card.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                definition.IsVisible = !definition.IsVisible;
                card.BackgroundColor = definition.IsVisible ? Color.FromArgb("#E8ECFD") : Colors.White;
            })
        });
        KeywordsLayout.Children.Add(card);
    }

    SetTab(true);
}

private void SetTab(bool summary)
{
    var primary = (Color)Application.Current!.Resources["Primary"];
    SummaryLabel.IsVisible = summary;
    KeywordsLayout.IsVisible = !summary;

    var keywordsHeader = this.FindByName<VerticalStackLayout>("KeywordsHeader");
    if (keywordsHeader != null)
        keywordsHeader.IsVisible = !summary;

    var soft = Color.FromArgb("#DDE3FB");
    var ink = Color.FromArgb("#14124A");
    SummaryTab.BackgroundColor = summary ? primary : soft;
    SummaryTab.TextColor = summary ? Colors.White : ink;
    KeywordsTab.BackgroundColor = summary ? soft : primary;
    KeywordsTab.TextColor = summary ? ink : Colors.White;
}

    

    private void OnSummaryTab(object sender, EventArgs e) => SetTab(true);
    private void OnKeywordsTab(object sender, EventArgs e) => SetTab(false);

    private async void OnQuizClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("quiz");
}
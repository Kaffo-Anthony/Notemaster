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

        KeywordsLayout.Children.Clear();
        foreach (var k in fiche.Keywords)
        {
            var definition = new Label
            {
                Text = k.Definition,
                FontSize = 13,
                TextColor = Colors.Black,
                IsVisible = false
            };
            var header = new Label
            {
                Text = $"{k.Name}  •  Importance : {k.Importance}",
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black
            };
            var card = new Border
            {
                Padding = 12,
                BackgroundColor = Color.FromArgb("#E8EBFA"),
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Content = new VerticalStackLayout { Spacing = 6, Children = { header, definition } }
            };
            card.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => definition.IsVisible = !definition.IsVisible)
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

        SummaryTab.BackgroundColor = summary ? primary : Color.FromArgb("#E8EBFA");
        SummaryTab.TextColor = summary ? Colors.White : Colors.Black;
        KeywordsTab.BackgroundColor = summary ? Color.FromArgb("#E8EBFA") : primary;
        KeywordsTab.TextColor = summary ? Colors.Black : Colors.White;
    }

    private void OnSummaryTab(object sender, EventArgs e) => SetTab(true);
    private void OnKeywordsTab(object sender, EventArgs e) => SetTab(false);

    private async void OnQuizClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("quiz");
}
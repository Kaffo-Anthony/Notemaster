using Notemaster.Models;

namespace Notemaster.Pages;

public partial class CoursesPage : ContentPage
{
    private readonly List<Course> _all = new()
    {
        new("Histoire de France", "24 fiches"),
        new("Biologie Cellulaire", "18 fiches"),
        new("Mathématiques - Algèbre", "32 fiches"),
        new("Philosophie - Lumières", "12 fiches"),
        new("Physique - Mouvement", "8 fiches"),
    };

    public CoursesPage()
    {
        InitializeComponent();
        Refresh("");
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => Refresh(e.NewTextValue ?? "");

    private void Refresh(string query)
    {
        var list = _all.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        CoursesView.ItemsSource = list;
        EmptyLabel.IsVisible = list.Count == 0;
    }

    private async void OnCreateClicked(object sender, EventArgs e)
        => await DisplayAlert("Créer un cours", "Fenêtre à venir.", "OK");
}
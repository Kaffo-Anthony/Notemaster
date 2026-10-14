using Notemaster.Models;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class CourseFichesPage : ContentPage
{
    public CourseFichesPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CourseTitle.Text = CourseStore.Current;
        Refresh("");
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => Refresh(e.NewTextValue ?? "");

    private void Refresh(string query)
    {
        var inCourse = FicheStore.All.Where(f => f.CourseName == CourseStore.Current).ToList();
        CourseCount.Text = inCourse.Count == 1 ? "1 fiche" : $"{inCourse.Count} fiches";

        var list = inCourse.Where(f => f.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        FichesView.ItemsSource = list;

        EmptyLabel.IsVisible = list.Count == 0;
        EmptyLabel.Text = inCourse.Count == 0
            ? "Aucune fiche dans ce cours.\nCommence par ajouter une note."
            : "Aucune fiche ne correspond à ta recherche.";
    }

    private async void OnFichePicked(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Fiche fiche) return;
        FichesView.SelectedItem = null;
        await DisplayAlert(fiche.Title, "Détail de la fiche à venir.", "OK");
    }

    private async void OnAddNoteClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("addnote");
}
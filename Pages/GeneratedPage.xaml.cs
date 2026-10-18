using Notemaster.Models;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class GeneratedPage : ContentPage
{
    public GeneratedPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var n = FicheStore.LastBatch.Count;
        CourseLabel.Text = $"Rattachées à : {NoteDraft.CourseName}";
        CountLabel.Text = $"{n} fiche{(n > 1 ? "s" : "")} générée{(n > 1 ? "s" : "")} à partir de la note";
        FichesView.ItemsSource = FicheStore.LastBatch;
    }

    protected override bool OnBackButtonPressed()
    {
        _ = Shell.Current.GoToAsync("//home");
        return true;
    }

    private async void OnFichePicked(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Fiche fiche) return;
        FichesView.SelectedItem = null;
        FicheStore.Selected = fiche;
        await Shell.Current.GoToAsync("fichedetail");
    }

    private async void OnViewCourseClicked(object sender, EventArgs e)
    {
        CourseStore.Current = NoteDraft.CourseName;
        await Shell.Current.GoToAsync("//home");
        await Shell.Current.GoToAsync("//courses");
        await Shell.Current.GoToAsync("coursefiches");
    }

    private async void OnDoneClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//home");
}
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class HomePage : ContentPage
{
    public HomePage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var courses = CourseStore.All.Count;
        var fiches = FicheStore.All.Count;
        GreetingLabel.Text = $"Bonjour, {UserSession.Name}";
        CoursesInfoLabel.Text = $"{courses} cours • {fiches} fiche{(fiches > 1 ? "s" : "")}";
    }

    private async void OnAddNoteClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("addnote");

    private async void OnCoursesTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//courses");
}
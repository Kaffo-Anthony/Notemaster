namespace Notemaster.Pages;

public partial class HomePage : ContentPage
{
    public HomePage() => InitializeComponent();

    private async void OnAddNoteClicked(object sender, EventArgs e)
        => await DisplayAlert("Ajouter une note", "Écran à venir.", "OK");

    private async void OnCoursesTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//courses");
}
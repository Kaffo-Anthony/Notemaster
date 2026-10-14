namespace Notemaster.Pages;

public partial class HomePage : ContentPage
{
    public HomePage() => InitializeComponent();

    private async void OnAddNoteClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("addnote");

    private async void OnCoursesTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//courses");
}
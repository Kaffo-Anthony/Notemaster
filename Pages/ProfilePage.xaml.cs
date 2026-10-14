namespace Notemaster.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage() => InitializeComponent();

    private async void OnLogoutClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//login");
}
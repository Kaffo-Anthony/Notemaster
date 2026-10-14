namespace Notemaster.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";

        if (!email.Contains('@') || password.Length == 0)
        {
            ErrorLabel.Text = "Email ou mot de passe invalide.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;
        await Shell.Current.GoToAsync("//home");
    
    }
    private async void OnForgotTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//recovery");
    private async void OnRegisterTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//register");

}
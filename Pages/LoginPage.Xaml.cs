using Notemaster.Services;

namespace Notemaster.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PasswordEntry.Text = "";
        ErrorLabel.IsVisible = false;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";

        if (!email.Contains('@') || password.Length == 0)
        {
            ShowError("Email ou mot de passe invalide.");
            return;
        }

        var account = AccountStore.Get(email);
        if (account == null || !AccountStore.Verify(email, password))
        {
            ShowError("Identifiants incorrects.");
            return;
        }

        ErrorLabel.IsVisible = false;
        UserSession.Email = email;
        UserSession.Name = account.Name;
        UserSession.School = account.School;
        await Shell.Current.GoToAsync("//home");
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private async void OnForgotTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//recovery");
    private async void OnRegisterTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//register");
}
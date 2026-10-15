using Notemaster.Services;

namespace Notemaster.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage() => InitializeComponent();

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var school = SchoolEntry.Text?.Trim() ?? "";
        var password = PasswordEntry.Text ?? "";

        string error = "";
        if (name == "" || school == "") error = "Nom et établissement obligatoires.";
        else if (!email.Contains('@')) error = "Email invalide.";
        else if (password.Length < 6) error = "Mot de passe : 6 caractères minimum.";
        else if (password != ConfirmEntry.Text) error = "Les mots de passe sont différents.";
        else if (!TermsCheck.IsChecked) error = "Acceptez les conditions d'utilisation.";
        else if (AccountStore.Exists(email)) error = "Un compte existe déjà avec cet email.";

        ErrorLabel.IsVisible = error != "";
        ErrorLabel.Text = error;
        if (error != "") return;

        AccountStore.Create(name, email, school, password);
        UserSession.Name = name;
        UserSession.Email = email;
        UserSession.School = school;
        await Shell.Current.GoToAsync("//home");
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//login");
}
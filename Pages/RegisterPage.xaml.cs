namespace Notemaster.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage() => InitializeComponent();

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        string error = "";
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(SchoolEntry.Text))
            error = "Nom et établissement obligatoires.";
        else if (!(EmailEntry.Text ?? "").Contains('@'))
            error = "Email invalide.";
        else if ((PasswordEntry.Text ?? "").Length < 6)
            error = "Mot de passe : 6 caractères minimum.";
        else if (PasswordEntry.Text != ConfirmEntry.Text)
            error = "Les mots de passe sont différents.";
        else if (!TermsCheck.IsChecked)
            error = "Acceptez les conditions d'utilisation.";

        ErrorLabel.IsVisible = error != "";
        ErrorLabel.Text = error;
        if (error == "") await Shell.Current.GoToAsync("//home");
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//login");
}
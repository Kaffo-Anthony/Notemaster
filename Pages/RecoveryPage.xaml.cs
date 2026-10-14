namespace Notemaster.Pages;

public partial class RecoveryPage : ContentPage
{
    public RecoveryPage() => InitializeComponent();

    private void OnSendClicked(object sender, EventArgs e)
    {
        bool valid = (EmailEntry.Text ?? "").Contains('@');
        MessageLabel.IsVisible = true;
        MessageLabel.TextColor = valid ? Colors.Green : Color.FromArgb("#DC2626");
        MessageLabel.Text = valid ? "Lien envoyé. Vérifiez votre boîte mail." : "Email invalide.";
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//login");
}
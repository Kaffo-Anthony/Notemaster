using Notemaster.Services;

namespace Notemaster.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NameLabel.Text = UserSession.Name;
        EmailLabel.Text = string.IsNullOrEmpty(UserSession.Email) ? "Email non renseigné" : UserSession.Email;
        SchoolLabel.Text = string.IsNullOrEmpty(UserSession.School) ? "Établissement non renseigné" : UserSession.School;
        CoursesCount.Text = CourseStore.All.Count.ToString();
        FichesCount.Text = FicheStore.All.Count.ToString();
        QuizCount.Text = UserSession.QuizzesDone.ToString();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        CourseStore.All.Clear();
        FicheStore.All.Clear();
        FicheStore.LastBatch = new();
        NoteDraft.Reset();
        await Shell.Current.GoToAsync("//login");
    }
}
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class ProcessingPage : ContentPage
{
    private static readonly string[] Steps =
        { "Analyse de la note", "Extraction des notions clés", "Génération des fiches" };

    public ProcessingPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = RunAsync();
    }

    protected override bool OnBackButtonPressed() => true;

    private async Task RunAsync()
    {
        LoadingSection.IsVisible = true;
        ErrorSection.IsVisible = false;
        SetStep(0);

        var result = await FicheGenerator.GenerateAsync(SetStep);

        if (result.Error != null)
        {
            LoadingSection.IsVisible = false;
            ErrorSection.IsVisible = true;
            ErrorLabel.Text = result.Error;
            return;
        }

        FicheStore.All.AddRange(result.Fiches!);
        FicheStore.LastBatch = result.Fiches!;
        DataStore.Save();
        await Shell.Current.GoToAsync("../generated");
    }

    private void SetStep(int current)
{
    var rows = new[] { Step1Row, Step2Row, Step3Row };
    var icons = new[] { Step1Icon, Step2Icon, Step3Icon };
    var labels = new[] { Step1Label, Step2Label, Step3Label };
    var badges = new[] { Step1Badge, Step2Badge, Step3Badge };

    for (int i = 0; i < 3; i++)
    {
        labels[i].Text = Steps[i];

        if (i < current)           
        {
            rows[i].BackgroundColor = Color.FromArgb("#DCFCE7");
            icons[i].Text = FluentUI.checkmark_circle_24_regular;
            icons[i].TextColor = Color.FromArgb("#15803D");
            badges[i].Text = "Terminé";
            badges[i].TextColor = Color.FromArgb("#15803D");
        }
        else if (i == current)      
        {
            rows[i].BackgroundColor = Color.FromArgb("#DDE3FB");
            icons[i].Text = FluentUI.arrow_sync_24_regular;
            icons[i].TextColor = Color.FromArgb("#3F4FC4");
            badges[i].Text = "En cours";
            badges[i].TextColor = Color.FromArgb("#3F4FC4");
        }
        else                        
        {
            rows[i].BackgroundColor = Color.FromArgb("#F3F4FA");
            icons[i].Text = FluentUI.clock_24_regular;
            icons[i].TextColor = Color.FromArgb("#6B7280");
            badges[i].Text = "En attente";
            badges[i].TextColor = Color.FromArgb("#6B7280");
        }
    }

    StepProgress.Progress = current / 3.0;
    PercentLabel.Text = $"{(int)(current / 3.0 * 100)} %";
}

    private async void OnRetryClicked(object sender, EventArgs e) => await RunAsync();

    private async void OnCancelClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
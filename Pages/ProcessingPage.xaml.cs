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
        var labels = new[] { Step1Label, Step2Label, Step3Label };
        for (int i = 0; i < 3; i++)
            labels[i].Text = (i < current ? "✓ " : i == current ? "… " : "○ ") + Steps[i];
    }

    private async void OnRetryClicked(object sender, EventArgs e) => await RunAsync();

    private async void OnCancelClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
using Notemaster.Models;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class QuizPage : ContentPage
{
    private List<QuizQuestion> _questions = new();
    private RadioButton[] _options;
    private int _index;
    private int _score;
    private bool _started;
    private bool _validated;

    public QuizPage()
    {
        InitializeComponent();
        _options = new[] { Opt0, Opt1, Opt2, Opt3 };
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(async () => await ConfirmQuit())
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_started) return;
        _started = true;
        await LoadQuizAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        _ = ConfirmQuit();
        return true;
    }

    private async Task ConfirmQuit()
    {
        if (!QuestionSection.IsVisible)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }
        if (await DisplayAlert("Quitter le quiz ?", "Si tu quittes maintenant, ta progression sera perdue.", "Quitter", "Annuler"))
            await Shell.Current.GoToAsync("..");
    }

    private void ShowSection(string name)
    {
        LoadingSection.IsVisible = name == "loading";
        ErrorSection.IsVisible = name == "error";
        QuestionSection.IsVisible = name == "question";
        ResultSection.IsVisible = name == "result";
    }

    private async Task LoadQuizAsync()
    {
        ShowSection("loading");
        try
        {
            _questions = await QuizGenerator.GenerateAsync(FicheStore.Selected!);
            StartQuiz();
        }
        catch
        {
            ShowSection("error");
        }
    }

    private void StartQuiz()
    {
        _index = 0;
        _score = 0;
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        var q = _questions[_index];
        _validated = false;

        ProgressLabel.Text = $"Question {_index + 1}/{_questions.Count}";
        QuizProgress.Progress = (double)_index / _questions.Count;
        QuestionLabel.Text = q.Text;

        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].IsChecked = false;
            _options[i].IsEnabled = true;
            _options[i].IsVisible = i < q.Options.Count;
            _options[i].Content = i < q.Options.Count ? q.Options[i] : "";
        }

        FeedbackBox.IsVisible = false;
        NextButton.IsVisible = false;
        ValidateButton.IsVisible = true;
        ValidateButton.IsEnabled = false;
        ShowSection("question");
    }

    private void OnOptionChecked(object sender, CheckedChangedEventArgs e)
        => ValidateButton.IsEnabled = !_validated && _options.Any(o => o.IsChecked);

    private void OnValidateClicked(object sender, EventArgs e)
    {
        var q = _questions[_index];
        int chosen = Array.FindIndex(_options, o => o.IsChecked);
        if (chosen < 0) return;

        _validated = true;
        bool ok = chosen == q.CorrectIndex;
        if (ok) _score++;

        foreach (var o in _options) o.IsEnabled = false;

        FeedbackBox.BackgroundColor = Color.FromArgb(ok ? "#D1FAE5" : "#FEE2E2");
        FeedbackLabel.Text = ok
            ? $"Correct !\n{q.Explanation}"
            : $"Incorrect. Bonne réponse : {q.Options[q.CorrectIndex]}\n{q.Explanation}";
        FeedbackBox.IsVisible = true;

        ValidateButton.IsVisible = false;
        NextButton.Text = _index == _questions.Count - 1 ? "Voir le résultat" : "Question suivante";
        NextButton.IsVisible = true;
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        if (_index == _questions.Count - 1)
        {
            ShowResult();
            return;
        }
        _index++;
        ShowQuestion();
    }

    private void ShowResult()
    {
        UserSession.QuizzesDone++;
        DataStore.Save();
        int percent = _score * 100 / _questions.Count;
        ScoreLabel.Text = $"{_score}/{_questions.Count}";

        if (percent >= 80)
        {
            ResultTitle.Text = "Excellent !";
            ResultMessage.Text = $"{percent}% de réussite. Tu maîtrises bien cette fiche.";
        }
        else if (percent >= 50)
        {
            ResultTitle.Text = "Bien joué";
            ResultMessage.Text = $"{percent}% de réussite. Encore un petit effort.";
        }
        else
        {
            ResultTitle.Text = "Résultat faible";
            ResultMessage.Text = $"{percent}% de réussite. Courage, tu peux t'améliorer !";
        }
        ShowSection("result");
    }

    private void OnRetryQuizClicked(object sender, EventArgs e) => StartQuiz();

    private async void OnRetryLoadClicked(object sender, EventArgs e) => await LoadQuizAsync();

    private async void OnBackToFicheClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
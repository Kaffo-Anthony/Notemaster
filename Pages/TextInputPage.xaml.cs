using Notemaster.Services;

namespace Notemaster.Pages;

public partial class TextInputPage : ContentPage
{
    public TextInputPage() => InitializeComponent();

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var len = e.NewTextValue?.Length ?? 0;
        CounterLabel.Text = $"{len} / 10000 caractères";
        ContinueButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {   NoteDraft.Reset();
        NoteDraft.Kind = "text";
        NoteDraft.Text = TextEditor.Text;
        NoteDraft.FilePath = null;
        NoteDraft.FileName = null;
        await Shell.Current.GoToAsync("preview");
    }

    private async void OnPickDocxClicked(object sender, EventArgs e)
    {
        var docx = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
            { DevicePlatform.WinUI, new[] { ".docx" } },
            { DevicePlatform.iOS, new[] { "org.openxmlformats.wordprocessingml.document" } },
            { DevicePlatform.MacCatalyst, new[] { "org.openxmlformats.wordprocessingml.document" } },
        });

        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Choisir un fichier Word",
            FileTypes = docx
        });
        if (file == null) return;
        NoteDraft.Reset();
        NoteDraft.Kind = "docx";
        NoteDraft.FilePath = file.FullPath;
        NoteDraft.FileName = file.FileName;
        NoteDraft.Text = null;
        await Shell.Current.GoToAsync("preview");
    }
}
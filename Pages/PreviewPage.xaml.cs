using Notemaster.Models;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class PreviewPage : ContentPage
{
    private const string CreateLabel = "+ Créer un cours";

    public PreviewPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();

        bool isPhoto = NoteDraft.Kind == "photo";
        PhotosSection.IsVisible = isPhoto;
        TextSection.IsVisible = !isPhoto;

        if (isPhoto)
            BuildThumbs();
        else if (NoteDraft.Kind == "text")
        {
            var t = NoteDraft.Text ?? "";
            PreviewText.Text = t.Length > 300 ? t[..300] + "…" : t;
        }
        else
            PreviewText.Text = "Fichier Word : " + NoteDraft.FileName;

        LoadCourses(null);
    }

    private void BuildThumbs()
    {
        ThumbsLayout.Children.Clear();

        for (int i = 0; i < NoteDraft.Photos.Count; i++)
        {
            int index = i;
            var thumb = new VerticalStackLayout { Spacing = 4, WidthRequest = 120 };

            thumb.Children.Add(new Image
            {
                Source = ImageSource.FromFile(NoteDraft.Photos[i]),
                WidthRequest = 120,
                HeightRequest = 150,
                Aspect = Aspect.AspectFill
            });

            var replace = new Button { Text = "Remplacer", FontSize = 12, HeightRequest = 36, Padding = new Thickness(4, 0) };
            replace.Clicked += async (s, e) => await ReplacePhoto(index);
            thumb.Children.Add(replace);

            var remove = new Button { Text = "Retirer", FontSize = 12, HeightRequest = 36, Padding = new Thickness(4, 0), BackgroundColor = Color.FromArgb("#DC2626") };
            remove.Clicked += async (s, e) => await RemovePhoto(index);
            thumb.Children.Add(remove);

            ThumbsLayout.Children.Add(thumb);
        }

        PagesLabel.Text = $"Tes pages ({NoteDraft.Photos.Count}/{NoteDraft.MaxPhotos})";
        AddPageButton.IsVisible = NoteDraft.Photos.Count < NoteDraft.MaxPhotos;
    }

    private async Task ReplacePhoto(int index)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null) return;
            NoteDraft.Photos[index] = await NoteDraft.SavePhotoAsync(photo);
            BuildThumbs();
        }
        catch (PermissionException)
        { await DisplayAlert("Permission refusée", "Autorise l'accès aux photos dans les paramètres.", "OK"); }
    }

    private async Task RemovePhoto(int index)
    {
        NoteDraft.Photos.RemoveAt(index);
        if (NoteDraft.Photos.Count == 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }
        BuildThumbs();
    }

    private async void OnAddPageClicked(object sender, EventArgs e)
    {
        int remaining = NoteDraft.MaxPhotos - NoteDraft.Photos.Count;
        if (remaining <= 0) return;

        var choice = await DisplayActionSheet("Ajouter une page", "Annuler", null,
            "Prendre une photo", "Choisir dans la galerie");
        try
        {
            if (choice == "Prendre une photo" && MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null) NoteDraft.Photos.Add(await NoteDraft.SavePhotoAsync(photo));
            }
            else if (choice == "Choisir dans la galerie")
            {
                var photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = remaining });
                if (photos != null)
                    foreach (var p in photos.Take(remaining))
                        NoteDraft.Photos.Add(await NoteDraft.SavePhotoAsync(p));
            }
        }
        catch (PermissionException)
        { await DisplayAlert("Permission refusée", "Autorise l'accès dans les paramètres.", "OK"); }

        BuildThumbs();
    }

    private void LoadCourses(string? select)
    {
        var items = CourseStore.All.Select(c => c.Name).ToList();
        items.Add(CreateLabel);
        CoursePicker.ItemsSource = items;
        if (select != null) CoursePicker.SelectedIndex = items.IndexOf(select);
    }

    private async void OnCourseChanged(object sender, EventArgs e)
    {
        var index = CoursePicker.SelectedIndex;

        if (index == CourseStore.All.Count)   // « + Créer un cours »
        {
            var name = await DisplayPromptAsync("Créer un cours", "Nom du cours", "Créer", "Annuler", "Ex : Maths - Chapitre 4");
            if (string.IsNullOrWhiteSpace(name))
            {
                CoursePicker.SelectedIndex = -1;
                return;
            }
            name = name.Trim();
            CourseStore.All.Add(new Course(name));
            LoadCourses(name);
            return;
        }

        GenerateButton.IsEnabled = index >= 0;
        HintLabel.IsVisible = index < 0;
    }

    private async void OnGenerateClicked(object sender, EventArgs e)
    {
        NoteDraft.CourseName = CoursePicker.SelectedItem?.ToString() ?? "";
        NoteDraft.Title = TitleEntry.Text?.Trim() ?? "";
        await Shell.Current.GoToAsync("processing");
        
    }
       
}
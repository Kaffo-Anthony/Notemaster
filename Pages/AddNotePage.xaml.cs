using Notemaster.Services;

namespace Notemaster.Pages;

public partial class AddNotePage : ContentPage
{
    public AddNotePage() => InitializeComponent();

    private async void OnCameraTapped(object sender, TappedEventArgs e)
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await DisplayAlert("Caméra", "Caméra non disponible sur cet appareil.", "OK");
            return;
        }
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;
            NoteDraft.Reset();
            NoteDraft.Kind = "photo";
            NoteDraft.Photos.Add(await NoteDraft.SavePhotoAsync(photo));
            await Shell.Current.GoToAsync("preview");
        }
        catch (PermissionException)
        { await DisplayAlert("Permission refusée", "Autorise la caméra dans les paramètres.", "OK"); }
    }

    private async void OnGalleryTapped(object sender, TappedEventArgs e)
    {
        try
        {
            var photos = await MediaPicker.Default.PickPhotosAsync(
                new MediaPickerOptions { SelectionLimit = NoteDraft.MaxPhotos, Title = "Choisis tes pages" });
            if (photos == null || photos.Count == 0) return;

            NoteDraft.Reset();
            NoteDraft.Kind = "photo";
            foreach (var p in photos.Take(NoteDraft.MaxPhotos))
                NoteDraft.Photos.Add(await NoteDraft.SavePhotoAsync(p));
            await Shell.Current.GoToAsync("preview");
        }
        catch (PermissionException)
        { await DisplayAlert("Permission refusée", "Autorise l'accès aux photos dans les paramètres.", "OK"); }
    }

    private async void OnTextTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("textinput");
}
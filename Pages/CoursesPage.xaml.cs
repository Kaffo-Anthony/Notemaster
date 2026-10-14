using Notemaster.Models;
using Notemaster.Services;

namespace Notemaster.Pages;

public partial class CoursesPage : ContentPage
{
    private readonly List<Course> _all = CourseStore.All;

    public CoursesPage()
    {
        InitializeComponent();
        Refresh("");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh("");
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => Refresh(e.NewTextValue ?? "");

    private void Refresh(string query)
    {
        var list = _all.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        CoursesView.ItemsSource = list;
        EmptyLabel.IsVisible = list.Count == 0;
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Créer un cours", "Nom du cours", "Créer", "Annuler", "Ex : Maths - Chapitre 4");
        if (string.IsNullOrWhiteSpace(name)) return;
        CourseStore.All.Add(new Course(name.Trim()));
        Refresh("");
    }
    
    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
    if (e.CurrentSelection.FirstOrDefault() is not Course course) return;
    CoursesView.SelectedItem = null;
    CourseStore.Current = course.Name;
    await Shell.Current.GoToAsync("coursefiches");
    }

}
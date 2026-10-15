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
        EmptyBox.IsVisible = list.Count == 0;

        if (_all.Count == 0)
        {
        EmptyTitle.Text = "Aucun cours pour le moment";
        EmptyText.Text = "Crée ton premier cours pour organiser tes fiches et révisions.";
        }
        else
        {
        EmptyTitle.Text = "Aucun résultat";
        EmptyText.Text = $"Aucun cours ne correspond à « {query} ».";
        }

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
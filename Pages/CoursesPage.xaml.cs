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
    CountLabel.Text = $"{_all.Count} cours";

    if (_all.Count == 0)
    {
        EmptyIcon.Text = FluentUI.book_open_24_regular;
        EmptyTitle.Text = "Aucun cours pour le moment";
        EmptyText.Text = "Crée ton premier cours pour organiser tes fiches et révisions.";
        ClearSearchButton.IsVisible = false;
    }
    else
    {
        EmptyIcon.Text = FluentUI.search_24_regular;
        EmptyTitle.Text = "Aucun résultat";
        EmptyText.Text = $"Aucun cours ne correspond à « {query} ». Essaie un autre mot-clé.";
        ClearSearchButton.IsVisible = true;
    }
    }

    private void OnCreateClicked(object sender, EventArgs e)
    {
         
    NewCourseEntry.Text = "";
    CreateError.IsVisible = false;
    CreateOverlay.IsVisible = true;
    NewCourseEntry.Focus();
    }
    private void OnCreateCancelClicked(object sender, EventArgs e) => CreateOverlay.IsVisible = false;
    private void OnCreateConfirmClicked(object sender, EventArgs e)
{
    var name = NewCourseEntry.Text?.Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        CreateError.Text = "Donne un nom à ton cours.";
        CreateError.IsVisible = true;
        return;
    }
    CourseStore.All.Add(new Course(name));
    DataStore.Save();
    CreateOverlay.IsVisible = false;
    SearchEntry.Text = "";
    Refresh("");
}

private void OnClearSearchClicked(object sender, EventArgs e) => SearchEntry.Text = "";



    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
    if (e.CurrentSelection.FirstOrDefault() is not Course course) return;
    CoursesView.SelectedItem = null;
    CourseStore.Current = course.Name;
    await Shell.Current.GoToAsync("coursefiches");
    }

}
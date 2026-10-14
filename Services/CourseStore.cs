using Notemaster.Models;

namespace Notemaster.Services;

public static class CourseStore
{
    public static string Current { get; set; } = "";

    public static List<Course> All { get; } = new()
    {
        new("Histoire de France"),
        new("Biologie Cellulaire"),
        new("Mathématiques - Algèbre"),
        new("Philosophie - Lumières"),
        new("Physique - Mouvement"),
    };
}
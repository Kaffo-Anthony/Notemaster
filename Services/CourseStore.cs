using Notemaster.Models;

namespace Notemaster.Services;

public static class CourseStore
{
    public static List<Course> All { get; } = new()
    {
        new("Histoire de France", "24 fiches"),
        new("Biologie Cellulaire", "18 fiches"),
        new("Mathématiques - Algèbre", "32 fiches"),
        new("Philosophie - Lumières", "12 fiches"),
        new("Physique - Mouvement", "8 fiches"),
    };
}
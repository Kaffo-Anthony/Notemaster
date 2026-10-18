using Notemaster.Services;

namespace Notemaster.Models;

public record Course(string Name)
{
    private static readonly string[] Palette = { "#3F4FC4", "#1F9D9A", "#6B4FC4", "#B7791F", "#C2410C", "#0F766E" };
    public Microsoft.Maui.Graphics.Color Accent =>
    Microsoft.Maui.Graphics.Color.FromArgb(Palette[Name.Sum(c => c) % Palette.Length]);
    public string Info
    {
        get
        {
            var n = FicheStore.All.Count(f => f.CourseName == Name);
            return n == 0 ? "Aucune fiche" : n == 1 ? "1 fiche" : $"{n} fiches";
        }
    }
}
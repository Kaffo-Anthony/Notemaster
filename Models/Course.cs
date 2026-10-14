using Notemaster.Services;

namespace Notemaster.Models;

public record Course(string Name)
{
    public string Info
    {
        get
        {
            var n = FicheStore.All.Count(f => f.CourseName == Name);
            return n == 0 ? "Aucune fiche" : n == 1 ? "1 fiche" : $"{n} fiches";
        }
    }
}
using Notemaster.Models;

namespace Notemaster.Services;

public static class FicheGenerator
{
    // VERSION PROVISOIRE : simule le traitement et produit des fiches de démonstration.
    // Sera remplacée par l'appel à l'IA.
    public static async Task<(List<Fiche>? Fiches, string? Error)> GenerateAsync(Action<int> onStep)
    {
        onStep(0);
        await Task.Delay(900);

        var error = CheckNote();
        if (error != null) return (null, error);

        onStep(1);
        await Task.Delay(900);

        onStep(2);
        await Task.Delay(900);

        var count = NoteDraft.Kind == "photo" ? NoteDraft.Photos.Count : 1;
        var baseTitle = string.IsNullOrWhiteSpace(NoteDraft.Title) ? "Note" : NoteDraft.Title.Trim();
        var fiches = new List<Fiche>();

        for (int i = 0; i < count; i++)
        {
            var title = count == 1 ? baseTitle : $"{baseTitle} - partie {i + 1}";
            fiches.Add(new Fiche(
                title,
                "[Démo] Résumé provisoire : le vrai contenu sera généré par l'IA à partir de ta note.",
                new List<Keyword>
                {
                    new("Exemple 1", "[Démo] Définition provisoire du mot-clé.", "Élevée"),
                    new("Exemple 2", "[Démo] Définition provisoire du mot-clé.", "Moyenne"),
                    new("Exemple 3", "[Démo] Définition provisoire du mot-clé.", "Faible"),
                    },
                NoteDraft.CourseName));
        }
        return (fiches, null);
    }

    private static string? CheckNote()
    {
        switch (NoteDraft.Kind)
        {
            case "text":
                if ((NoteDraft.Text?.Trim().Length ?? 0) < 20)
                    return "Le texte est vide ou trop court pour générer des fiches à partir de la note.";
                break;
            case "docx":
                if (string.IsNullOrEmpty(NoteDraft.FilePath) || !File.Exists(NoteDraft.FilePath))
                    return "Le fichier Word est illisible. Vérifie le fichier et réessaie.";
                break;
            case "photo":
                if (NoteDraft.Photos.Count == 0 || NoteDraft.Photos.Any(p => !File.Exists(p)))
                    return "Une photo est illisible. Remplace-la par une photo plus nette.";
                break;
        }
        return null;
    }
}
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Notemaster.Models;

namespace Notemaster.Services;

public static class FicheGenerator
{
    private const long MaxTotalImageBytes = 14 * 1024 * 1024;

    private const string SystemPrompt = """
        Tu es un assistant qui transforme des notes de cours en fiches de révision.
        Réponds UNIQUEMENT avec un objet JSON valide, sans aucun texte autour, au format :
        {"fiches":[{"title":"...","summary":"...","keywords":[{"name":"...","definition":"...","importance":"Élevée|Moyenne|Faible"}]}]}
        Règles :
        - Découpe la note en plusieurs fiches seulement si elle couvre plusieurs notions distinctes. Une courte note donne une seule fiche. Maximum 6 fiches.
        - Chaque résumé est structuré (court, clair) et fidèle à la note, sans rien inventer.
        - 4 à 8 mots-clés par fiche, avec une définition courte tirée de la note.
        - Écris dans la langue de la note.
        - Si la note est illisible ou ne contient pas de cours, réponds exactement : {"error":"illisible"}
        """;

    public static async Task<(List<Fiche>? Fiches, string? Error)> GenerateAsync(Action<int> onStep)
    {
        onStep(0);
        var (parts, prepError) = await BuildPartsAsync();
        if (prepError != null) return (null, prepError);

        onStep(1);
        if (!GeminiClient.IsConfigured) return await DemoAsync(onStep);

        var (text, error) = await GeminiClient.GenerateJsonAsync(SystemPrompt, parts!);
        if (error != null) return (null, error);

        onStep(2);
        return Parse(text!);
    }

    private static async Task<(List<object>? Parts, string? Error)> BuildPartsAsync()
    {
        var parts = new List<object>();
        var hint = string.IsNullOrWhiteSpace(NoteDraft.Title) ? "" : $" Titre indiqué par l'étudiant : {NoteDraft.Title}.";

        switch (NoteDraft.Kind)
        {
            case "photo":
                if (NoteDraft.Photos.Count == 0 || NoteDraft.Photos.Any(p => !File.Exists(p)))
                    return (null, "Une photo est illisible. Remplace-la par une photo plus nette.");

                long total = 0;
                foreach (var path in NoteDraft.Photos)
                {
                    var bytes = await File.ReadAllBytesAsync(path);
                    total += bytes.Length;
                    if (total > MaxTotalImageBytes)
                        return (null, "Les photos sont trop lourdes (14 Mo maximum au total). Retire une page ou prends des photos plus légères.");

                    parts.Add(new { inline_data = new { mime_type = MediaType(path), data = Convert.ToBase64String(bytes) } });
                }
                parts.Add(new { text = "Voici les pages de ma note de cours. Génère les fiches de révision." + hint });
                break;

            case "text":
                var text = NoteDraft.Text?.Trim() ?? "";
                if (text.Length < 20)
                    return (null, "Le texte est vide ou trop court pour générer des fiches à partir de la note.");
                parts.Add(new { text = "Voici ma note de cours :\n\n" + Limit(text) + "\n\nGénère les fiches de révision." + hint });
                break;

            case "docx":
                var docx = NoteDraft.FilePath != null && File.Exists(NoteDraft.FilePath) ? ReadDocx(NoteDraft.FilePath) : null;
                if (docx == null)
                    return (null, "Le fichier Word est illisible. Vérifie le fichier et réessaie.");
                if (docx.Trim().Length < 20)
                    return (null, "Le fichier Word est vide ou trop court pour générer des fiches à partir de la note.");
                parts.Add(new { text = "Voici ma note de cours :\n\n" + Limit(docx) + "\n\nGénère les fiches de révision." + hint });
                break;

            default:
                return (null, "Aucune note à traiter.");
        }
        return (parts, null);
    }

    private static string Limit(string text) => text.Length > 10000 ? text[..10000] : text;

    private static string MediaType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".gif" => "image/gif",
        _ => "image/jpeg"
    };

    private static string? ReadDocx(string path)
    {
        try
        {
            using var zip = ZipFile.OpenRead(path);
            var entry = zip.GetEntry("word/document.xml");
            if (entry == null) return null;

            using var stream = entry.Open();
            var doc = XDocument.Load(stream);
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            var sb = new StringBuilder();
            foreach (var p in doc.Descendants(w + "p"))
                sb.AppendLine(string.Concat(p.Descendants(w + "t").Select(t => t.Value)));
            return sb.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static (List<Fiche>? Fiches, string? Error) Parse(string responseText)
    {
        const string unreadable = "La réponse de l'IA est illisible. Réessaie.";
        try
        {
            int a = responseText.IndexOf('{'), b = responseText.LastIndexOf('}');
            if (a < 0 || b <= a) return (null, unreadable);

            using var json = JsonDocument.Parse(responseText[a..(b + 1)]);
            var root = json.RootElement;

            if (root.TryGetProperty("error", out _))
                return (null, "La note est illisible ou ne contient pas de cours exploitable. Reprends-la avec plus de netteté.");

            var list = new List<Fiche>();
            foreach (var f in root.GetProperty("fiches").EnumerateArray())
            {
                var keywords = f.GetProperty("keywords").EnumerateArray()
                    .Select(k => new Keyword(
                        k.GetProperty("name").GetString() ?? "",
                        k.GetProperty("definition").GetString() ?? "",
                        k.GetProperty("importance").GetString() ?? "Moyenne"))
                    .ToList();

                list.Add(new Fiche(
                    f.GetProperty("title").GetString() ?? "Fiche",
                    f.GetProperty("summary").GetString() ?? "",
                    keywords,
                    NoteDraft.CourseName));
            }
            return list.Count == 0 ? (null, unreadable) : (list, null);
        }
        catch
        {
            return (null, unreadable);
        }
    }

    private static async Task<(List<Fiche>? Fiches, string? Error)> DemoAsync(Action<int> onStep)
    {
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
}
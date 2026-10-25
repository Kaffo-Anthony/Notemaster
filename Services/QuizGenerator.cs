using System.Text.Json;
using Notemaster.Models;

namespace Notemaster.Services;

public static class QuizGenerator
{
    private const string SystemPrompt = """
        Tu es un assistant pédagogique. À partir d'une fiche de révision, génère un quiz de 10 questions à choix multiples.
        Réponds UNIQUEMENT avec un objet JSON valide, sans aucun texte autour, au format :
        {"questions":[{"text":"...","options":["...","...","...","..."],"correctIndex":0,"explanation":"..."}]}
        Règles :
        - 4 options par question, une seule bonne réponse, correctIndex va de 0 à 3.
        - Questions fidèles à la fiche, sans rien inventer. Explication courte.
        - Varie la position de la bonne réponse.
        - Écris dans la langue de la fiche.
        """;

    public static async Task<List<QuizQuestion>> GenerateAsync(Fiche fiche)
    {
        if (!GeminiClient.IsConfigured) return await DemoAsync(fiche);

        var content = $"Titre : {fiche.Title}\nRésumé : {fiche.Summary}\nMots-clés :\n"
            + string.Join("\n", fiche.Keywords.Select(k => $"- {k.Name} : {k.Definition}"));

        var (text, error) = await GeminiClient.GenerateJsonAsync(SystemPrompt, new List<object> { new { text = content } });
        if (error != null) throw new InvalidOperationException(error);

        int a = text!.IndexOf('{'), b = text.LastIndexOf('}');
        if (a < 0 || b <= a) throw new InvalidOperationException("Réponse illisible.");

        using var json = JsonDocument.Parse(text[a..(b + 1)]);
        var list = new List<QuizQuestion>();

        foreach (var q in json.RootElement.GetProperty("questions").EnumerateArray())
        {
            var options = q.GetProperty("options").EnumerateArray().Select(o => o.GetString() ?? "").ToList();
            var correct = q.GetProperty("correctIndex").GetInt32();
            if (options.Count < 2 || correct < 0 || correct >= options.Count) continue;

            list.Add(new QuizQuestion(
                q.GetProperty("text").GetString() ?? "",
                options,
                correct,
                q.GetProperty("explanation").GetString() ?? ""));
        }

        if (list.Count == 0) throw new InvalidOperationException("Quiz vide.");
        return list;
    }

    private static async Task<List<QuizQuestion>> DemoAsync(Fiche fiche)
    {
        await Task.Delay(1500);
        var list = new List<QuizQuestion>();
        for (int i = 1; i <= 3; i++)
        {
            list.Add(new QuizQuestion(
                $"[Démo] Question {i} sur « {fiche.Title} »",
                new List<string> { "Réponse A", "Réponse B", "Réponse C", "Réponse D" },
                i % 4,
                "[Démo] Explication provisoire de la bonne réponse."));
        }
        return list;
    }
}
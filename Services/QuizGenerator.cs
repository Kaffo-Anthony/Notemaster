using Notemaster.Models;

namespace Notemaster.Services;

public static class QuizGenerator
{
    // VERSION PROVISOIRE : sera remplacée par l'appel à l'IA.
    public static async Task<List<QuizQuestion>> GenerateAsync(Fiche fiche)
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
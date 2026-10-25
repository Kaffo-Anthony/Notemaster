using System.Text;
using System.Text.Json;

namespace Notemaster.Services;

public static class GeminiClient
{
    // Nom du modèle : vérifie-le dans AI Studio (bouton « Get code ») s'il est refusé.
    private const string Model = "gemini-3.1-flash-lite";
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(90) };

    public static bool IsConfigured
        => !string.IsNullOrWhiteSpace(Secrets.GeminiKey) && Secrets.GeminiKey != "COLLE_TA_CLE_ICI";

    public static async Task<(string? Text, string? Error)> GenerateJsonAsync(string systemPrompt, List<object> parts)
    {
        try
        {
            var body = JsonSerializer.Serialize(new
            {
                systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
                contents = new[] { new { role = "user", parts } },
                generationConfig = new { responseMimeType = "application/json", maxOutputTokens = 8000, temperature = 0.3 }
            });

            using var req = new HttpRequestMessage(HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent");
            req.Headers.Add("x-goog-api-key", Secrets.GeminiKey);
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var res = await Http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode) return (null, ErrorFor((int)res.StatusCode));

            using var doc = JsonDocument.Parse(json);
            var text = doc.RootElement.GetProperty("candidates")[0]
                .GetProperty("content").GetProperty("parts")[0]
                .GetProperty("text").GetString();
            return (text, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return (null, "Connexion impossible. Vérifie ta connexion internet et réessaie.");
        }
        catch (Exception)
        {
            return (null, "La réponse de l'IA est illisible. Réessaie.");
        }
    }

    private static string ErrorFor(int status) => status switch
    {
        400 => "Requête refusée par l'IA. Vérifie la clé API et la taille de la note.",
        401 or 403 => "Accès au service d'IA refusé : vérifie la clé API.",
        404 => "Modèle d'IA introuvable. Vérifie le nom du modèle dans GeminiClient.cs.",
        429 => "Limite gratuite atteinte. Réessaie dans une minute.",
        >= 500 => "Le service d'IA a un problème temporaire. Réessaie dans un instant.",
        _ => "Le traitement a échoué. Réessaie."
    };
}
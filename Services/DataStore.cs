using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Notemaster.Models;

namespace Notemaster.Services;

public static class DataStore
{
    public record Snapshot(List<string> Courses, List<Fiche> Fiches, int QuizzesDone);

    private static string PathFor(string email)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
        return Path.Combine(FileSystem.AppDataDirectory, $"data_{Convert.ToHexString(bytes)[..16]}.json");
    }

    public static void Load(string email)
    {
        CourseStore.All.Clear();
        FicheStore.All.Clear();
        FicheStore.LastBatch = new();
        UserSession.QuizzesDone = 0;

        var path = PathFor(email);
        if (!File.Exists(path)) return;

        try
        {
            var snap = JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(path));
            if (snap == null) return;
            foreach (var name in snap.Courses) CourseStore.All.Add(new Course(name));
            FicheStore.All.AddRange(snap.Fiches);
            UserSession.QuizzesDone = snap.QuizzesDone;
        }
        catch
        {
            // fichier illisible : on repart de zéro
        }
    }

    public static void Save()
    {
        var email = UserSession.Email;
        if (string.IsNullOrEmpty(email)) return;

        var snap = new Snapshot(
            CourseStore.All.Select(c => c.Name).ToList(),
            FicheStore.All.ToList(),
            UserSession.QuizzesDone);

        File.WriteAllText(PathFor(email), JsonSerializer.Serialize(snap));
    }
}
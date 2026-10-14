namespace Notemaster.Models;

public record Fiche(string Title, string Summary, List<string> Keywords, string CourseName)
{
    public string Preview => Summary.Length > 90 ? Summary[..90] + "…" : Summary;
    public string KeywordsInfo => $"Résumé • {Keywords.Count} mots-clés";
}
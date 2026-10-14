using Notemaster.Models;

namespace Notemaster.Services;

public static class FicheStore
{
    public static List<Fiche> All { get; } = new();

    public static List<Fiche> LastBatch { get; set; } = new();
}

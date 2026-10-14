namespace Notemaster.Services;

public static class NoteDraft
{
    public const int MaxPhotos = 4;

    public static string Kind = "";   // "photo", "text" ou "docx"
    public static List<string> Photos { get; } = new();
    public static string? FilePath;
    public static string? FileName;
    public static string? Text;
    public static string CourseName = "";
    public static string Title = "";

    public static void Reset()
    {
        Kind = "";
        Photos.Clear();
        FilePath = null;
        FileName = null;
        Text = null;
        CourseName = "";
        Title = "";
    }

    public static async Task<string> SavePhotoAsync(FileResult photo)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}_{photo.FileName}");
        using var src = await photo.OpenReadAsync();
        using var dst = File.Create(path);
        await src.CopyToAsync(dst);
        return path;
    }
}
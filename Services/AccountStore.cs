using System.Security.Cryptography;
using System.Text.Json;

namespace Notemaster.Services;

public static class AccountStore
{
    public record Account(string Name, string School, string Salt, string Hash);

    private static string Key(string email) => "acct_" + email.Trim().ToLowerInvariant();

    public static bool Exists(string email) => Preferences.ContainsKey(Key(email));

    public static Account? Get(string email)
    {
        var json = Preferences.Get(Key(email), "");
        return json == "" ? null : JsonSerializer.Deserialize<Account>(json);
    }

    public static void Create(string name, string email, string school, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var account = new Account(name, school, Convert.ToBase64String(salt), Hash(password, salt));
        Preferences.Set(Key(email), JsonSerializer.Serialize(account));
    }

    public static bool Verify(string email, string password)
    {
        var account = Get(email);
        if (account == null) return false;
        var salt = Convert.FromBase64String(account.Salt);
        return Hash(password, salt) == account.Hash;
    }

    private static string Hash(string password, byte[] salt)
        => Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32));
}
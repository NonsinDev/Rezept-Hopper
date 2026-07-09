using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Rezept_Hopper.Data;
using Rezept_Hopper.Data.Models;

namespace Rezept_Hopper.Services;

public class AuthService(AppDbContext db)
{
    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return null;
        return VerifyPassword(password, user.PasswordHash, user.Salt) ? user : null;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string username, string password)
    {
        if (await db.Users.AnyAsync(u => u.Username == username))
            return (false, "Benutzername ist bereits vergeben.");

        var (hash, salt) = HashPassword(password);
        db.Users.Add(new User { Username = username, PasswordHash = hash, Salt = salt });
        await db.SaveChangesAsync();
        return (true, null);
    }

    private static (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(32);
        var salt = Convert.ToBase64String(saltBytes);
        var hash = ComputeHash(password, salt);
        return (hash, salt);
    }

    private static bool VerifyPassword(string password, string hash, string salt)
        => ComputeHash(password, salt) == hash;

    private static string ComputeHash(string password, string salt)
    {
        var bytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            Convert.FromBase64String(salt),
            iterations: 200_000,
            HashAlgorithmName.SHA256,
            outputLength: 32);
        return Convert.ToBase64String(bytes);
    }
}

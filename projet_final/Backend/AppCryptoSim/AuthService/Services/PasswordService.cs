namespace AuthService.Services;

public static class PasswordService
{
    private const int WorkFactor = 11;

    public static string HashPassword(string motDePasse)
    {
        return BCrypt.Net.BCrypt.HashPassword(motDePasse, WorkFactor);
    }

    public static bool VerifyPassword(string motDePasse, string motDePasseHashe)
    {
        return BCrypt.Net.BCrypt.Verify(motDePasse, motDePasseHashe);
    }
}

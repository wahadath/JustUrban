using JustUrban.Models;
using Microsoft.AspNetCore.Identity;

namespace JustUrban.Auth
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<AppUser> Hasher = new();

        public static string Hash(string password) =>
            Hasher.HashPassword(new AppUser(), password);

        public static bool Verify(AppUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return false;
            }

            var result = Hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result != PasswordVerificationResult.Failed;
        }
    }
}

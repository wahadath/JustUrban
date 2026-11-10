using Microsoft.AspNetCore.Http;

namespace JustUrban.Storage
{
    public static class KycStorage
    {
        public const string RequestPath = "/uploads";
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxBytes = 5 * 1024 * 1024;

        public static string RootPath(string contentRootPath) =>
            Path.GetFullPath(Path.Combine(contentRootPath, "..", "uploads"));

        public static async Task<string> SaveAsync(IFormFile file, string contentRootPath, string prefix)
        {
            var error = Validate(file);
            if (error != null)
            {
                throw new InvalidOperationException(error);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var folder = Path.Combine(RootPath(contentRootPath), "kyc");
            Directory.CreateDirectory(folder);

            var name = $"{prefix}-{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, name);
            await using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream);
            return $"{RequestPath}/kyc/{name}";
        }

        public static string? Validate(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return "Please upload a photo.";
            }

            if (file.Length > MaxBytes)
            {
                return "Each photo must be 5 MB or smaller.";
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                return "Photos must be JPG, PNG, or WEBP.";
            }

            return null;
        }
    }
}

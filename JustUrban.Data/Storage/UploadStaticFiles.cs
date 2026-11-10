using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace JustUrban.Storage
{
    public static class UploadStaticFiles
    {
        public static void UseKycUploads(this WebApplication app)
        {
            var root = KycStorage.RootPath(app.Environment.ContentRootPath);
            Directory.CreateDirectory(Path.Combine(root, "kyc"));
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(root),
                RequestPath = KycStorage.RequestPath
            });
        }
    }
}

using JustUrban.Models;
using Microsoft.AspNetCore.Http;

namespace JustUrban.Auth
{
    public static class StaffSession
    {
        public const string AdminApp = "Admin";
        public const string VendorApp = "Vendor";

        public static bool IsLoggedIn(HttpRequest request, string app) =>
            request.Cookies[Cookie(app, "IsLoggedIn")] == "true";

        public static int? UserId(HttpRequest request, string app)
        {
            if (int.TryParse(request.Cookies[Cookie(app, "UserId")], out var id) && id > 0)
            {
                return id;
            }

            return null;
        }

        public static int? VendorId(HttpRequest request)
        {
            if (int.TryParse(request.Cookies[Cookie(VendorApp, "VendorId")], out var id) && id > 0)
            {
                return id;
            }

            return null;
        }

        public static string DisplayName(HttpRequest request, string app) =>
            request.Cookies[Cookie(app, "UserName")] ?? "Account";

        public static void SignIn(HttpRequest request, HttpResponse response, string app, AppUser user, int? vendorId = null)
        {
            var options = CookieOptions(request);
            response.Cookies.Append(Cookie(app, "IsLoggedIn"), "true", options);
            response.Cookies.Append(Cookie(app, "UserId"), user.Id.ToString(), options);
            response.Cookies.Append(Cookie(app, "UserName"), user.FullName ?? user.Email ?? "Account", options);
            response.Cookies.Append(Cookie(app, "Role"), user.Role, options);
            if (vendorId.HasValue)
            {
                response.Cookies.Append(Cookie(app, "VendorId"), vendorId.Value.ToString(), options);
            }
        }

        public static void SignOut(HttpRequest request, HttpResponse response, string app)
        {
            var options = DeleteOptions(request);
            response.Cookies.Delete(Cookie(app, "IsLoggedIn"), options);
            response.Cookies.Delete(Cookie(app, "UserId"), options);
            response.Cookies.Delete(Cookie(app, "UserName"), options);
            response.Cookies.Delete(Cookie(app, "Role"), options);
            response.Cookies.Delete(Cookie(app, "VendorId"), options);
        }

        private static string Cookie(string app, string name) => $"JustUrban.{app}.{name}";

        private static CookieOptions CookieOptions(HttpRequest request) => new()
        {
            Path = "/",
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(14)
        };

        private static CookieOptions DeleteOptions(HttpRequest request) => new()
        {
            Path = "/",
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = request.IsHttps
        };
    }
}

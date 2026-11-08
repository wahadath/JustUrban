using JustUrban.Models;

namespace JustUrban.Services
{
    public static class AuthSession
    {
        public const string LoggedInCookie = "IsLoggedIn";
        public const string UserIdCookie = "UserId";
        public const string UserPhoneCookie = "UserPhone";
        public const string UserNameCookie = "UserName";

        public static bool IsLoggedIn(HttpRequest request) =>
            request.Cookies[LoggedInCookie] == "true";

        public static int? UserId(HttpRequest request)
        {
            if (int.TryParse(request.Cookies[UserIdCookie], out var id) && id > 0)
            {
                return id;
            }

            return null;
        }

        public static CookieOptions CookieOptions(HttpRequest request) => new()
        {
            Path = "/",
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };

        public static CookieOptions DeleteOptions(HttpRequest request) => new()
        {
            Path = "/",
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = request.IsHttps
        };

        public static void SignIn(HttpRequest request, HttpResponse response, AppUser user)
        {
            var options = CookieOptions(request);
            response.Cookies.Append(LoggedInCookie, "true", options);
            response.Cookies.Append(UserIdCookie, user.Id.ToString(), options);
            response.Cookies.Append(UserPhoneCookie, user.PhoneDisplay, options);
            response.Cookies.Append(UserNameCookie, user.FullName ?? string.Empty, options);
        }

        public static void SignOut(HttpRequest request, HttpResponse response)
        {
            var options = DeleteOptions(request);
            response.Cookies.Delete(LoggedInCookie, options);
            response.Cookies.Delete(UserIdCookie, options);
            response.Cookies.Delete(UserPhoneCookie, options);
            response.Cookies.Delete(UserNameCookie, options);
        }

        public static string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            return new string(phone.Where(char.IsDigit).ToArray());
        }
    }
}

using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (AuthSession.IsLoggedIn(Request))
            {
                return RedirectToPage("/Profile");
            }

            TempData["OpenLogin"] = "true";
            return RedirectToPage("/Index");
        }
    }
}

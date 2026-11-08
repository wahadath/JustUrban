using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            AuthSession.SignOut(Request, Response);
            return RedirectToPage("/Index");
        }
    }
}

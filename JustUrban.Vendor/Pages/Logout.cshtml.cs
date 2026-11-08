using JustUrban.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Vendor.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            StaffSession.SignOut(Request, Response, StaffSession.VendorApp);
            return RedirectToPage("/Login");
        }
    }
}

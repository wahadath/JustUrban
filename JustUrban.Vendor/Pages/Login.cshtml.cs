using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Vendor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Email { get; set; } = "vendor@justurban.local";

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (StaffSession.IsLoggedIn(Request, StaffSession.VendorApp))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _db.Users
                .Include(u => u.VendorProfile)
                .FirstOrDefaultAsync(u => u.Email == Email && u.Role == UserRoles.Vendor && u.IsActive);

            if (user == null || user.VendorProfile == null || !PasswordHelper.Verify(user, Password))
            {
                ErrorMessage = "Invalid vendor email or password.";
                return Page();
            }

            StaffSession.SignIn(Request, Response, StaffSession.VendorApp, user, user.VendorProfile.Id);
            return RedirectToPage("/Index");
        }
    }
}

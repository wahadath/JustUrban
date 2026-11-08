using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Email { get; set; } = "admin@justurban.local";

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email == Email && u.Role == UserRoles.Admin && u.IsActive);

            if (user == null || !PasswordHelper.Verify(user, Password))
            {
                ErrorMessage = "Invalid admin email or password.";
                return Page();
            }

            StaffSession.SignIn(Request, Response, StaffSession.AdminApp, user);
            return RedirectToPage("/Index");
        }
    }
}

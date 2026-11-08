using JustUrban.Data;
using JustUrban.Models;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _db;

        public ProfileModel(AppDbContext db)
        {
            _db = db;
        }

        public AppUser? Account { get; set; }

        public IActionResult OnGet()
        {
            var userId = AuthSession.UserId(Request);
            if (!AuthSession.IsLoggedIn(Request) || userId == null)
            {
                TempData["OpenLogin"] = "true";
                return RedirectToPage("/Index");
            }

            Account = _db.Users.Find(userId.Value);
            if (Account == null)
            {
                AuthSession.SignOut(Request, Response);
                TempData["OpenLogin"] = "true";
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}

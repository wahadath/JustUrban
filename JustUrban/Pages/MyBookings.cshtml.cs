using JustUrban.Data;
using JustUrban.Models;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class MyBookingsModel : PageModel
    {
        private readonly AppDbContext _db;

        public MyBookingsModel(AppDbContext db)
        {
            _db = db;
        }

        public List<Booking> Bookings { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!AuthSession.IsLoggedIn(Request))
            {
                TempData["OpenLogin"] = "true";
                TempData["LoginError"] = "Log in to see your bookings.";
                return RedirectToPage("/Index");
            }

            var phone = Request.Cookies[AuthSession.UserPhoneCookie];
            var query = _db.Bookings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var digits = AuthSession.NormalizePhone(phone);
                query = query.Where(b =>
                    b.ContactNumber == phone ||
                    (digits.Length > 0 && b.ContactNumber.Contains(digits)));
            }

            Bookings = query.OrderByDescending(b => b.CreatedAt).ToList();
            return Page();
        }
    }
}

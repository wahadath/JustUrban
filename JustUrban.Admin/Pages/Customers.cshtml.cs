using JustUrban.Auth;
using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.Pages
{
    public class CustomersModel : PageModel
    {
        private readonly AppDbContext _db;

        public CustomersModel(AppDbContext db)
        {
            _db = db;
        }

        public List<AppUser> Customers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            Customers = await _db.Users
                .Where(u => u.Role == UserRoles.Customer)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return Page();
        }
    }
}

using JustUrban.Auth;
using JustUrban.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JustUrban.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public int TotalBookings { get; set; }
        public int PendingAssignment { get; set; }
        public int Completed { get; set; }
        public int VendorCount { get; set; }
        public int CustomerCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!StaffSession.IsLoggedIn(Request, StaffSession.AdminApp))
            {
                return RedirectToPage("/Login");
            }

            TotalBookings = await _db.Bookings.CountAsync();
            PendingAssignment = await _db.Bookings.CountAsync(b =>
                b.VendorId == null && b.Status != "Cancelled" && b.Status != "Completed");
            Completed = await _db.Bookings.CountAsync(b => b.Status == "Completed");
            VendorCount = await _db.Vendors.CountAsync();
            CustomerCount = await _db.Users.CountAsync(u => u.Role == "Customer");
            return Page();
        }
    }
}

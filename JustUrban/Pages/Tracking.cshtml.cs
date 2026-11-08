using JustUrban.Data;
using JustUrban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class TrackingModel : PageModel
    {
        private readonly AppDbContext _db;

        public TrackingModel(AppDbContext db)
        {
            _db = db;
        }

        public Booking? Booking { get; set; }

        public IActionResult OnGet(int id)
        {
            Booking = _db.Bookings.Find(id);
            if (Booking == null)
            {
                return RedirectToPage("/MyBookings");
            }

            return Page();
        }

        public IActionResult OnPostCancel(int id)
        {
            var booking = _db.Bookings.Find(id);
            if (booking == null)
            {
                return RedirectToPage("/MyBookings");
            }

            if (booking.CanModify)
            {
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();
            }

            return RedirectToPage("/Tracking", new { id });
        }
    }
}

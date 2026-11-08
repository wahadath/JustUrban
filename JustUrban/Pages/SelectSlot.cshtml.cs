using JustUrban.Data;
using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class SelectSlotModel : PageModel
    {
        private readonly AppDbContext _db;

        public SelectSlotModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int? BookingId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Service { get; set; }

        public bool IsReschedule => BookingId.HasValue;

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(Service))
            {
                TempData["ServiceName"] = BookingFlow.DisplayServiceName(Service);
            }

            BookingFlow.KeepBookingTempData(TempData);
        }

        public IActionResult OnPost(DateTime SelectedDate, string SelectedHour, string SelectedMinute, string SelectedPeriod)
        {
            if (string.IsNullOrEmpty(SelectedHour) || string.IsNullOrEmpty(SelectedMinute) || string.IsNullOrEmpty(SelectedPeriod))
            {
                ModelState.AddModelError(string.Empty, "Please fill out all time selection elements.");
                return Page();
            }

            int hour = int.Parse(SelectedHour);
            int minute = int.Parse(SelectedMinute);

            if (SelectedPeriod == "PM" && hour < 12) hour += 12;
            if (SelectedPeriod == "AM" && hour == 12) hour = 0;

            TimeSpan chosenTime = new TimeSpan(hour, minute, 0);
            TimeSpan openingTime = new TimeSpan(8, 0, 0);
            TimeSpan closingTime = new TimeSpan(20, 0, 0);

            if (chosenTime < openingTime || chosenTime > closingTime)
            {
                ModelState.AddModelError("TimeValidationError", "Bookings are only allowed between 08:00 AM and 08:00 PM. Please select an operational slot.");
                return Page();
            }

            string fullTimeFormatString = $"{SelectedHour}:{SelectedMinute} {SelectedPeriod}";

            if (BookingId.HasValue)
            {
                var booking = _db.Bookings.Find(BookingId.Value);
                if (booking == null)
                {
                    return RedirectToPage("/MyBookings");
                }

                if (!booking.CanModify)
                {
                    return RedirectToPage("/Tracking", new { id = booking.Id });
                }

                booking.SlotDate = SelectedDate.Date;
                booking.SlotTime = fullTimeFormatString;
                booking.Status = "Rescheduled";
                booking.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();

                return RedirectToPage("/Tracking", new { id = booking.Id });
            }

            TempData["ServiceName"] = TempData["ServiceName"] ?? BookingFlow.DisplayServiceName(Service);
            TempData["UserPickedDate"] = SelectedDate.ToString("yyyy-MM-dd");
            TempData["UserPickedTime"] = fullTimeFormatString;
            BookingFlow.KeepBookingTempData(TempData);

            return RedirectToPage("/Billing");
        }
    }
}

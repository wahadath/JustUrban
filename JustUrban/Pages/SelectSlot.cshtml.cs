using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class SelectSlotModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost(DateTime selectedDate, string selectedTime)
        {
            if (string.IsNullOrEmpty(selectedTime) || selectedDate == DateTime.MinValue)
            {
                ModelState.AddModelError(string.Empty, "Please choose a valid date and timing slot.");
                return Page();
            }

            // TempData holds appointment info to display on our final redirect target screen
            TempData["UserBookingDate"] = selectedDate.ToString("dd dddd MMM, yyyy");
            TempData["UserBookingTime"] = selectedTime;

            // Route user smoothly to a completed success summary screen next
            return RedirectToPage("/Billing");
        }
    }
}

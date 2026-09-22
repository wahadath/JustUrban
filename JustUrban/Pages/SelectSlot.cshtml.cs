using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace JustUrban.Pages
{
    public class SelectSlotModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost(DateTime SelectedDate, string SelectedHour, string SelectedMinute, string SelectedPeriod)
        {
            if (string.IsNullOrEmpty(SelectedHour) || string.IsNullOrEmpty(SelectedMinute) || string.IsNullOrEmpty(SelectedPeriod))
            {
                ModelState.AddModelError(string.Empty, "Please fill out all time selection elements.");
                return Page();
            }

            // 1. Parse the 12-hour components into a functional TimeSpan object for calculation
            int hour = int.Parse(SelectedHour);
            int minute = int.Parse(SelectedMinute);

            // Convert standard 12-hour values to 24-hour markers for simple checking
            if (SelectedPeriod == "PM" && hour < 12) hour += 12;
            if (SelectedPeriod == "AM" && hour == 12) hour = 0;

            TimeSpan chosenTime = new TimeSpan(hour, minute, 0);

            // Define active operating guidelines (08:00 AM to 08:00 PM)
            TimeSpan openingTime = new TimeSpan(8, 0, 0);  // 8:00 AM
            TimeSpan closingTime = new TimeSpan(20, 0, 0); // 8:00 PM

            // 2. Perform validation boundary checks
            if (chosenTime < openingTime || chosenTime > closingTime)
            {
                ModelState.AddModelError("TimeValidationError", "Bookings are only allowed between 08:00 AM and 08:00 PM. Please select an operational slot.");
                return Page();
            }

            // 3. Stash clean operational values for your pipeline if validation passes
            string fullTimeFormatString = $"{SelectedHour}:{SelectedMinute} {SelectedPeriod}";
            TempData["UserPickedDate"] = SelectedDate.ToString("yyyy-MM-dd");
            TempData["UserPickedTime"] = fullTimeFormatString;

            return RedirectToPage("/Billing");
        }
    }
}

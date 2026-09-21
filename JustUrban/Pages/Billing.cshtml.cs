using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class BillingModel : PageModel
    {
        public void OnGet()
        {
            // Code block handles loading initial request states
        }

        public IActionResult OnPost(string customerName, string contactNumber, string city, string pincode, string streetAddress)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TempData captures values safely across the workflow payment gateway pass phase redirect
            TempData["CustomerName"] = customerName;
            TempData["AmountDue"] = "49";

            // Navigates smoothly to a subsequent mock payment validation screen panel
            return RedirectToPage("/payment");
        }
    }
}

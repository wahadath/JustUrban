using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class paymentModel : PageModel
    {
        public void OnGet()
        {
            // Page initialization
        }

        public IActionResult OnPost(string paymentMethod)
        {
            // Explicitly tracking our target price point
            const decimal amount = 49.00m;

            try
            {
                if (paymentMethod == "Online")
                {
                    // SIMULATION: In real-life, you would call your Payment Gateway API here.
                    // For testing purposes, we will simulate a successful digital authorization:
                    bool isGatewayTransactionSuccessful = true;

                    if (isGatewayTransactionSuccessful)
                    {
                        TempData["PopupType"] = "Success";
                        TempData["PopupMessage"] = "Online Payment of ₹49.00 received successfully! Your booking is secured.";
                    }
                    else
                    {
                        TempData["PopupType"] = "Failure";
                        TempData["PopupMessage"] = "The bank rejected your online transaction. Please check your balance and try again.";
                    }
                }
                else if (paymentMethod == "Offline")
                {
                    // SIMULATION: Save record to your local database file as 'Pay at Desk / COD'
                    // Database.SaveOrder(amount, "Offline", "Pending");

                    TempData["PopupType"] = "Success";
                    TempData["PopupMessage"] = "Booking confirmed! Please keep exactly ₹49.00 cash ready at your slot timing.";
                }
            }
            catch (System.Exception ex)
            {
                // Fallback catch-all error handling
                TempData["PopupType"] = "Failure";
                TempData["PopupMessage"] = $"An unexpected system error occurred: {ex.Message}";
            }

            // Reload the same page so the system can safely render the pop-up notification modal
            return RedirectToPage();
        }
    }
}

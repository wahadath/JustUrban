using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;

namespace JustUrban.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Initial render state logic
        }

        // Action triggered by your modal phone form submission
        public IActionResult OnPostSendOtp(string PhoneNumber)
        {
            // Fallback safety check for null parameters
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                return Page();
            }

            // SIMULATION: If the user provides your specified mock input "1234"
            if (PhoneNumber.Trim() == "1234")
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = true, // Secures cookie accessibility profiles
                    IsEssential = true
                };

                // Append the authorization state marker flags
                Response.Cookies.Append("IsLoggedIn", "true", options);

                // Redirect straight back to your index page to reload the layout with the user icon visible
                return RedirectToPage("/Index");
            }

            // Fallback if anything else is entered (Keeps modal workflow open)
            return Page();
        }

        // Action triggered when hitting the "Log Out" item under your new profile icon dropdown menu selection
        public IActionResult OnPostLogout()
        {
            // Revoke authentication markers by clearing cookies
            Response.Cookies.Delete("IsLoggedIn");
            return RedirectToPage("/Index");
        }
    }
}

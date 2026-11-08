using JustUrban.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JustUrban.Pages
{
    public class BookingModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Service { get; set; }

        public string ServiceDisplayName { get; private set; } = "Home Makeover";

        public void OnGet()
        {
            ServiceDisplayName = BookingFlow.DisplayServiceName(Service);
            TempData["ServiceName"] = ServiceDisplayName;
            TempData.Keep("ServiceName");
        }
    }
}

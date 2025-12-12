using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YemekTarifleri.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public string AdminName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // --- YETKÝLENDÝRME KONTROLÜ (KRÝTÝK) ---
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            var username = HttpContext.Session.GetString("Username");

            // Kullanýcý oturum açmamýþsa veya Admin deðilse giriþe yönlendir
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            AdminName = username ?? "Yönetici";

            return Page();
        }
    }
}
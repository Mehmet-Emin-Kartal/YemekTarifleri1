using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YemekTarifleri.Pages
{
    public class LogoutModel : PageModel
    {
        // Logout iþlemi OnGet ile tetiklenir
        public IActionResult OnGet()
        {
            // Session'daki tüm kullanýcý verilerini (UserId, Username, IsAdmin) temizle
            HttpContext.Session.Clear();

            // Kullanýcýyý ana sayfaya yönlendir
            return RedirectToPage("/Index");
        }
    }
}
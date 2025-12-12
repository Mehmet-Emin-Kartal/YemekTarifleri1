using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using YemekTarifleri; // Modelimizi kullanabilmek için

namespace YemekTarifleri.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [Display(Name = "Kullanýcý Adý")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Þifre")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Beni Hatýrla")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
            // Eðer kullanýcý zaten giriþ yapmýþsa ana sayfaya yönlendir (Ýsteðe baðlý)
            if (HttpContext.Session.GetString("UserId") != null)
            {
                RedirectToPage("./Index");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Kullanýcýyý bul
            var user = _context.Users.SingleOrDefault(u => u.Username == Input.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanýcý adý veya þifre.");
                return Page();
            }

            // 2. Þifreyi kontrol et (Hashlenmiþ þifre ile)
            var inputHash = HashPassword(Input.Password);
            if (user.PasswordHash != inputHash)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanýcý adý veya þifre.");
                return Page();
            }

            // 3. Baþarýlý Giriþ: Oturum (Session) oluþtur
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

            // 4. Yetkiye göre yönlendirme (Admin giriþi ayrý olacak)
            if (user.IsAdmin)
            {
                // Admin ise Admin Paneline yönlendir
                return RedirectToPage("/Admin/Index");
            }
            else
            {
                // Normal kullanýcý ise Ana Sayfaya yönlendir
                return RedirectToPage("./Index");
            }
        }

        // --- ÞÝFRE HASHLEME METODU (Register sayfasýndaki ile ayný olmalý) ---
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
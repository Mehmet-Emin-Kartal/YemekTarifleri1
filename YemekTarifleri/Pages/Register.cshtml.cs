using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using YemekTarifleri; // Model Ad Alanýný ekledik

namespace YemekTarifleri.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: DB baðlantýsýný alýr
        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Input Modelini arayüze baðlar
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // Arka plan modeli tanýmý
        public class InputModel
        {
            [Required]
            [MinLength(3, ErrorMessage = "Kullanýcý adý en az 3 karakter olmalýdýr.")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr.")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Þifre ve onay þifresi eþleþmiyor.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            // Sayfa yüklendiðinde
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kullanýcý adý veya e-posta kontrolü
            if (_context.Users.Any(u => u.Username == Input.Username || u.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "Bu kullanýcý adý veya e-posta zaten kayýtlý.");
                return Page();
            }

            // Þifreyi hash'leme 
            var passwordHash = HashPassword(Input.Password);

            var newUser = new User
            {
                Username = Input.Username,
                Email = Input.Email,
                PasswordHash = passwordHash,
                IsAdmin = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Login");
        }

        // Þifre Hashleme Metodu
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using YemekTarifleri.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace YemekTarifleri.Pages.Admin.Recipes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Dosya yükleme için gerekli

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // Kategori seçimi için Dropdown listesi
        public SelectList CategoryList { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Tarif Adý zorunludur.")]
            [StringLength(100)]
            [Display(Name = "Tarif Adý")]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Açýklama zorunludur.")]
            [Display(Name = "Kýsa Açýklama")]
            [DataType(DataType.MultilineText)]
            public string ShortDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "Malzemeler zorunludur.")]
            [Display(Name = "Malzemeler")]
            [DataType(DataType.MultilineText)]
            public string Ingredients { get; set; } = string.Empty;

            [Required(ErrorMessage = "Talimatlar zorunludur.")]
            [Display(Name = "Hazýrlanýþ Talimatlarý")]
            [DataType(DataType.MultilineText)]
            public string Instructions { get; set; } = string.Empty;

            [Required(ErrorMessage = "Zorluk seviyesi zorunludur.")]
            [Display(Name = "Zorluk Seviyesi")]
            public string Difficulty { get; set; } = "Orta";

            [Required(ErrorMessage = "Hazýrlýk süresi zorunludur.")]
            [Display(Name = "Hazýrlýk Süresi")]
            public string PreparationTime { get; set; } = "30 Dakika";

            [Required(ErrorMessage = "Mutfak Türü zorunludur.")]
            [Display(Name = "Mutfak Türü")]
            public string CuisineType { get; set; } = "Türk";

            [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
            [Display(Name = "Kategori")]
            public int CategoryId { get; set; }

            [Display(Name = "Tarif Resmi Yükle")]
            [DataType(DataType.Upload)]
            public IFormFile? ImageFile { get; set; } // Yüklenen resmi tutar
        }

        // Zorluk ve Süreler için sabit listeler (Dropdown'lar için)
        public List<string> Difficulties { get; } = new List<string> { "Kolay", "Orta", "Zor" };
        public List<string> PreparationTimes { get; } = new List<string> { "15 Dakika", "30 Dakika", "1 Saat", "1 Günden Fazla" };
        public List<string> CuisineTypes { get; } = new List<string> { "Türk", "Ýtalyan", "Asya", "Meksika", "Diðer" };


        private async Task LoadSelectListsAsync()
        {
            // Kategori Dropdown'unu doldur
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            CategoryList = new SelectList(categories, "Id", "Name");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // YETKÝLENDÝRME KONTROLÜ
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Dosya yükleme baþarýlý olsa bile, diðer model hatalarýný kontrol et
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Dosya Yükleme Ýþlemi Baþlangýcý
            string imagePath = string.Empty;
            if (Input.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "recipes");

                // Klasör yoksa oluþtur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(fileStream);
                }

                // Yalnýzca /images/recipes/resim_adi.jpg yolunu veritabanýna kaydet
                imagePath = Path.Combine("/images/recipes", uniqueFileName).Replace('\\', '/');
            }

            // --- USER ID BULMA ---
            // Tarif yazarýný (Admin) bulma
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int authorId))
            {
                // Admin ID'si bulunamazsa hata ver veya varsayýlan ata
                TempData["ErrorMessage"] = "Yazar kimliði bulunamadý. Lütfen tekrar giriþ yapýn.";
                await LoadSelectListsAsync();
                return Page();
            }

            // Tarif nesnesini oluþtur
            var recipe = new Recipe
            {
                Title = Input.Title,
                ShortDescription = Input.ShortDescription,
                Ingredients = Input.Ingredients,
                Instructions = Input.Instructions,
                Difficulty = Input.Difficulty,
                PreparationTime = Input.PreparationTime,
                CuisineType = Input.CuisineType,
                CategoryId = Input.CategoryId,
                ImagePath = imagePath,
                AuthorId = authorId, // Yazar ID'sini atama
                AverageRating = 0.0, // Yeni tarif olduðu için baþlangýç puaný 0
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
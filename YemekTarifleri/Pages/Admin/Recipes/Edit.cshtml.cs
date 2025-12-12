using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace YemekTarifleri.Pages.Admin.Recipes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [BindProperty]
        public string? ExistingImagePath { get; set; }

        [BindProperty]
        public int RecipeId { get; set; }

        public SelectList CategoryList { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Tarif Adý zorunludur.")]
            [StringLength(100)]
            [Display(Name = "Tarif Adý")]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Kýsa açýklama zorunludur.")]
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
            public string Difficulty { get; set; } = string.Empty;

            [Required(ErrorMessage = "Hazýrlýk süresi zorunludur.")]
            [Display(Name = "Hazýrlýk Süresi")]
            public string PreparationTime { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mutfak Türü zorunludur.")]
            [Display(Name = "Mutfak Türü")]
            public string CuisineType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
            [Display(Name = "Kategori")]
            public int CategoryId { get; set; }

            [Display(Name = "Yeni Resim Yükle")]
            [DataType(DataType.Upload)]
            public IFormFile? ImageFile { get; set; }
        }

        public List<string> Difficulties { get; } = new List<string> { "Kolay", "Orta", "Zor" };
        public List<string> PreparationTimes { get; } = new List<string> { "15 Dakika", "30 Dakika", "1 Saat", "1 Günden Fazla" };
        public List<string> CuisineTypes { get; } = new List<string> { "Türk", "Ýtalyan", "Asya", "Meksika", "Diðer" };

        private async Task LoadSelectListsAsync()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            CategoryList = new SelectList(categories, "Id", "Name");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            if (id == null) return NotFound();

            var recipe = await _context.Recipes.FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null) return NotFound();

            RecipeId = recipe.Id;
            Input.Title = recipe.Title;
            Input.ShortDescription = recipe.ShortDescription;
            Input.Ingredients = recipe.Ingredients;
            Input.Instructions = recipe.Instructions;
            Input.Difficulty = recipe.Difficulty;
            Input.PreparationTime = recipe.PreparationTime;
            Input.CuisineType = recipe.CuisineType;
            Input.CategoryId = recipe.CategoryId;
            ExistingImagePath = recipe.ImagePath;

            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            var recipeToUpdate = await _context.Recipes.FindAsync(RecipeId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            // --- DOSYA YÜKLEME VE SÝLME ÝÞLEMÝ ---
            string newImagePath = ExistingImagePath ?? string.Empty;

            if (Input.ImageFile != null)
            {
                // Eski resmi sil
                if (!string.IsNullOrEmpty(ExistingImagePath))
                {
                    // WebRootPath'ten eski dosya yolunu oluþtur
                    string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, ExistingImagePath!.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Yeni resmi yükle
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
                // Veritabanýna kaydedilecek URL yolunu oluþtur
                newImagePath = Path.Combine("/images/recipes", uniqueFileName).Replace('\\', '/');
            }
            // --- DOSYA YÜKLEME SONU ---

            // Verileri güncelle
            recipeToUpdate.Title = Input.Title;
            recipeToUpdate.ShortDescription = Input.ShortDescription;
            recipeToUpdate.Ingredients = Input.Ingredients;
            recipeToUpdate.Instructions = Input.Instructions;
            recipeToUpdate.Difficulty = Input.Difficulty;
            recipeToUpdate.PreparationTime = Input.PreparationTime;
            recipeToUpdate.CuisineType = Input.CuisineType;
            recipeToUpdate.CategoryId = Input.CategoryId;
            recipeToUpdate.ImagePath = newImagePath;

            // Yazar ID'sinin güncellenmesini engelle
            _context.Entry(recipeToUpdate).Property(x => x.AuthorId).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Recipes.Any(e => e.Id == RecipeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
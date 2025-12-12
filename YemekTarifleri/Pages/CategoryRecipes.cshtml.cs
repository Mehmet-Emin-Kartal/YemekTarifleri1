using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;

namespace YemekTarifleri.Pages
{
    public class CategoryRecipesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CategoryRecipesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string CategoryName { get; set; } = string.Empty;
        public IList<Recipe> RecipeList { get; set; } = default!;

        // OnGet: URL'den gelen kategori ID'sine göre tarifleri çekme
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return RedirectToPage("/Index");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            CategoryName = category.Name;

            // Kategori ID'sine ait tüm tarifleri çekme
            RecipeList = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Author)
                .Where(r => r.CategoryId == id)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return Page();
        }
    }
}
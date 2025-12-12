using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;

namespace YemekTarifleri.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Recipe> RecipeList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Recipes != null)
            {
                // Tüm tarifleri, kategori adý ve yazar bilgisi dahil çekiyoruz
                RecipeList = await _context.Recipes
                    .Include(r => r.Category)
                    .Include(r => r.Author)
                    .OrderByDescending(r => r.Id)
                    .Take(6) // Ana sayfada ilk 6 tarifi gösterelim
                    .ToListAsync();
            }
        }
    }
}

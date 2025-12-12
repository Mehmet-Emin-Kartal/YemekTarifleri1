using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;

namespace YemekTarifleri.Pages.Admin.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listelenecek Tarifleri tutar
        public IList<Recipe> Recipe { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // --- YETKÝLENDÝRME KONTROLÜ ---
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            // Tüm tarifleri, ait olduklarý Kategori bilgisini de dahil ederek çekiyoruz.
            if (_context.Recipes != null)
            {
                Recipe = await _context.Recipes
                    .Include(r => r.Category) // Kategori adýný tabloda gösterebilmek için gerekli.
                    .OrderByDescending(r => r.Id) // En son ekleneni baþta göster
                    .ToListAsync();
            }

            return Page();
        }
    }
}

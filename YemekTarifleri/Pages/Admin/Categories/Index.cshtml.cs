using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri; // Category modelini kullanmak için
using YemekTarifleri.Models;

namespace YemekTarifleri.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // --- YETKÝLENDÝRME KONTROLÜ ---
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            if (_context.Categories != null)
            {
                Category = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }

            return Page();
        }
    }
}
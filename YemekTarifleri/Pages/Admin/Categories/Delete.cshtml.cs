using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri;
using YemekTarifleri.Models;

namespace YemekTarifleri.Pages.Admin.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category != null)
            {
                // 1. KONTROL: ALT KATEGORÝ KONTROLÜ
                if (category.ChildCategories != null && category.ChildCategories.Any())
                {
                    TempData["ErrorMessage"] = $"'{category.Name}' kategorisi silinemez. Lütfen önce {category.ChildCategories.Count} adet alt kategorisini silin veya taþýyýn.";
                    return RedirectToPage("./Index");
                }

                Category = category;
                _context.Categories.Remove(Category);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
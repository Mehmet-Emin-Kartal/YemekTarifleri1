using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;
using System.Linq;
using System.Threading.Tasks;

namespace YemekTarifleri.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // --- YETKÝLENDÝRME KONTROLÜ ---
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }
            // --- YETKÝLENDÝRME KONTROLÜ SONU ---

            if (id == null)
            {
                return NotFound();
            }

            // ID'ye göre kategoriyi çekme
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            Category = category;

            // Kategori listesini ana kategori seçimi için hazýrlama (ParentId)
            // Edit sayfasýnda, düzenlenen kategori kendini ana kategori olarak seçememeli.
            ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Categories.Where(c => c.Id != Category.Id).OrderBy(c => c.Name),
                "Id",
                "Name"
            );

            return Page();
        }

        // Güncelleme iþlemi
        public async Task<IActionResult> OnPostAsync()
        {
            // Model validasyonu (Gerekirse)
            if (!ModelState.IsValid)
            {
                // Hata varsa ParentId listesini tekrar yükle ve sayfaya dön
                ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    _context.Categories.Where(c => c.Id != Category.Id).OrderBy(c => c.Name),
                    "Id",
                    "Name"
                );
                return Page();
            }

            // Entity'yi veritabanýnda deðiþtirilmiþ olarak iþaretle
            _context.Attach(Category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.Id))
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

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
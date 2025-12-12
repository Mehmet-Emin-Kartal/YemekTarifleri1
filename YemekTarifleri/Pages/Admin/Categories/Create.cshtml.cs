using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri;
using YemekTarifleri.Models;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifleri.Pages.Admin.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public SelectList ParentCategoryList { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Kategori adý zorunludur.")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Kategori adý 3 ile 50 karakter arasýnda olmalýdýr.")]
            [Display(Name = "Kategori Adý")]
            public string Name { get; set; } = string.Empty;

            [Display(Name = "Üst Kategori (Opsiyonel)")]
            public int? ParentCategoryId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var isAdminString = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdminString) || isAdminString.ToLower() != "true")
            {
                return RedirectToPage("/Login");
            }

            var categories = await _context.Categories.ToListAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = null, Text = "— Üst Kategori Seçilmesin —" }
            };
            items.AddRange(categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }));

            ParentCategoryList = new SelectList(items, "Value", "Text");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var category = new Category
            {
                Name = Input.Name,
                ParentCategoryId = Input.ParentCategoryId > 0 ? Input.ParentCategoryId : null,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
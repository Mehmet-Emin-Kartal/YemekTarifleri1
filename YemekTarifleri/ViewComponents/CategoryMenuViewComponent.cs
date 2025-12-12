using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;
using System.Linq;
using System.Threading.Tasks;

namespace YemekTarifleri.ViewComponents
{
    // Razor sayfasında @await Component.InvokeAsync("CategoryMenu") olarak çağrılacak.
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Tüm kategorileri hiyerarşik yapı (ParentCategory) dahil çekiyoruz.
            var categories = await _context.Categories
                                           .Include(c => c.ChildCategories)
                                           .OrderBy(c => c.Name)
                                           .ToListAsync();

            // Sadece ana kategorileri (ParentCategoryId null olanları) alıyoruz.
            var mainCategories = categories.Where(c => c.ParentCategoryId == null).ToList();

            return View(mainCategories);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YemekTarifleri.Models;
using System.ComponentModel.DataAnnotations;

namespace YemekTarifleri.Pages
{
    public class RecipeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RecipeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Recipe Recipe { get; set; } = default!;
        public IList<Comment> Comments { get; set; } = default!;

        [BindProperty]
        public CommentInputModel CommentInput { get; set; } = new CommentInputModel();

        public class CommentInputModel
        {
            [Required(ErrorMessage = "Yorum içeriði boþ olamaz.")]
            [StringLength(500)]
            [Display(Name = "Yorumunuz")]
            public string Content { get; set; } = string.Empty;

            [Required]
            public int RecipeId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Recipe = await _context.Recipes
                .Include(r => r.Author) // SÝZÝN RECIPE.CS MODELÝNÝZE UYARLANDI
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Recipe == null) return NotFound();

            await LoadCommentsAsync(Recipe.Id);

            CommentInput.RecipeId = Recipe.Id;

            return Page();
        }

        public async Task<IActionResult> OnPostAddCommentAsync()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "Yorum yapabilmek için lütfen giriþ yapýn.";
                return RedirectToPage(new { id = CommentInput.RecipeId });
            }

            if (!ModelState.IsValid)
            {
                Recipe = await _context.Recipes.Include(r => r.Author).Include(r => r.Category).FirstOrDefaultAsync(m => m.Id == CommentInput.RecipeId);
                await LoadCommentsAsync(CommentInput.RecipeId);
                return Page();
            }

            var newComment = new Comment
            {
                Content = CommentInput.Content,
                RecipeId = CommentInput.RecipeId,
                UserId = userId // SÝZÝN COMMENT.CS MODELÝNÝZE UYARLANDI
                // CommentDate Modelinizde olmadýðý için eklenmiyor
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorumunuz baþarýyla eklendi.";
            return RedirectToPage(new { id = CommentInput.RecipeId });
        }

        private async Task LoadCommentsAsync(int recipeId)
        {
            Comments = await _context.Comments
               .Include(c => c.User) // SÝZÝN COMMENT.CS MODELÝNÝZE UYARLANDI
               .Where(c => c.RecipeId == recipeId)
               .ToListAsync();
        }
    }
}
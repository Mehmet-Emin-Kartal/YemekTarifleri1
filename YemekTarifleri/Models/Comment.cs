using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekTarifleri // Projenizin KESİN Ad Alanı
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty; // Yorum içeriği

        // --- İLİŞKİLER (FOREIGN KEYS) ---

        // Yorum hangi tarife ait?
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        // Yorumu kim yaptı? (Düzenleme/Silme yetkisi için zorunlu)
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
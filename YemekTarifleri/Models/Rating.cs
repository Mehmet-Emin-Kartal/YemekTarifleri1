using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekTarifleri // Projenizin KESİN Ad Alanı
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        [Range(1, 5)] // Puan sadece 1 ile 5 arasında olmalı
        public int Score { get; set; }

        // --- İLİŞKİLER (FOREIGN KEYS) ---

        // Puan hangi tarife ait?
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        // Puanı kim verdi? (Her kullanıcı bir tarife bir kez puan verebilir kuralı için zorunlu)
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
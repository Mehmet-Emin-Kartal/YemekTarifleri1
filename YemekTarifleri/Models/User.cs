using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YemekTarifleri;


namespace YemekTarifleri // Projenizin KESİN Ad Alanı
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Admin Yetkisi (Yetkilendirme Kuralı için)
        public bool IsAdmin { get; set; } = false;

        // Navigasyon Özellikleri (CS8618 çözümü için new List<>() zorunlu)
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
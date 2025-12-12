using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YemekTarifleri;
using YemekTarifleri.Models;

namespace YemekTarifleri // Projenizin KESİN Ad Alanı
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        // --- TEMEL BİLGİLER ---
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Kısa açıklama zorunludur.")] // <-- BU SATIRLARI EKLEYİN
        [Display(Name = "Kısa Açıklama")]
        public string ShortDescription { get; set; } = string.Empty; // <-- BU SATIRLARI EKLEYİN

        [Required]
        public string Ingredients { get; set; } = string.Empty; // Malzemeler (Madde listesi olarak gösterilecek)

        [Required]
        public string Instructions { get; set; } = string.Empty;

        // --- RESİM YÜKLEME ---
        // Video yerine Resim yükleme yolu (wwwroot/images/recipes/...)
        public string ImagePath { get; set; } = string.Empty;

        // --- FİLTRELEME VE PUANLAMA ALANLARI ---

        public double AverageRating { get; set; } = 0.0;
        public bool IsFeatured { get; set; } = false;

        [Required]
        public string Difficulty { get; set; } = string.Empty; // Zorluk Seviyesi (Örn: Kolay, Orta, Zor)

        [Required]
        public string PreparationTime { get; set; } = string.Empty; // Hazırlık Süresi (Örn: 30 Dakika, 1 Saat)

        [Required]
        public string CuisineType { get; set; } = string.Empty; // Mutfak Türü (Örn: Türk, İtalyan, Asya)


        // --- İLİŞKİLER (FOREIGN KEYS) ---

        // Kategori Bağlantısı
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // Yazar Bağlantısı (Admin/Üye yetkilendirmesi için)
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User? Author { get; set; }

        // Yorumlar ve Puanlar Koleksiyonları
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
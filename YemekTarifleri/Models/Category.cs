using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekTarifleri.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı zorunludur.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Kategori hiyerarşisi için
        public int? ParentCategoryId { get; set; }

        // Navigation Properties:

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; }

        // Bir üst kategorinin birden fazla alt kategorisi olabilir. (Daha önce eklemiştik)
        public ICollection<Category>? ChildCategories { get; set; }

        // BU KRİTİK SATIRI EKLE/DÜZELT: Bu kategoriye ait olan tariflerin listesi
        public ICollection<Recipe>? Recipes { get; set; } // Hata bu satırın eksikliğinden kaynaklanıyordu.


        // Constructor
        public Category()
        {
            ChildCategories = new HashSet<Category>();
            Recipes = new HashSet<Recipe>();
        }
    }
}
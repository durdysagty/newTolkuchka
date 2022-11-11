using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string PartNo { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NewPrice { get; set; }
        public bool NotInUse { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsNew { get; set; }
        public bool OnOrder { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int? LineId { get; set; }
        public Line Line { get; set; }
        public int? ModelId { get; set; }
        public Model Model { get; set; }
        public int? WarrantyId { get; set; }
        public Warranty Warranty { get; set; }
        public ICollection<CategoryProductAdLink> CategoryProductAdLinks { get; set; }
        public ICollection<ProductSpecsValue> ProductSpecsValues { get; set; }
        public ICollection<ProductSpecsValueMod> ProductSpecsValueMods { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<Wish> Wishes { get; set; }
    }
}

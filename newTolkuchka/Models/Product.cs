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

        public int CategoryId { get; set; } // to remove
        public Category Category { get; set; } // to remove
        public int TypeId { get; set; } // to remove
        public Type Type { get; set; } // to remove
        public int BrandId { get; set; } // to remove
        public Brand Brand { get; set; } // to remove
        public int? LineId { get; set; } // to remove
        public Line Line { get; set; } // to remove
        public int? ModelId { get; set; }
        public Model Model { get; set; }
        public int? WarrantyId { get; set; } // to remove
        public Warranty Warranty { get; set; } // to remove
        public ICollection<CategoryProductAdLink> CategoryProductAdLinks { get; set; } // to remove
        public ICollection<ProductSpecsValue> ProductSpecsValues { get; set; }
        public ICollection<ProductSpecsValueMod> ProductSpecsValueMods { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<Wish> Wishes { get; set; }
    }
}

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
        [MaxLength(1000)]
        public string DescRu { get; set; }

        [MaxLength(1000)]
        public string DescEn { get; set; }

        [MaxLength(1000)]
        public string DescTm { get; set; }
        public int? ModelId { get; set; }
        public Model Model { get; set; }
        public ICollection<ProductSpecsValue> ProductSpecsValues { get; set; }
        public ICollection<ProductSpecsValueMod> ProductSpecsValueMods { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<Wish> Wishes { get; set; }
    }
}

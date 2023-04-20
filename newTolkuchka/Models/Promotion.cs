using Microsoft.EntityFrameworkCore.Metadata.Internal;
using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public enum Tp { Discount, QuantityDiscount, QuantityFree, ProductFree, Set, SetDiscount, SpecialSetDiscount } // type, Discount - a line discount, QuantityDiscount - buy N quantity and get a discount, QuantityFree - buy N quantity get 1 for free, ProductFree - buy N quantity of the given product and get 1 subject for free, Set - buy all products in that set and get 1 subject product for free, SetDiscount - buy all products in that set and get discount for all of them, SpecialSetDiscount - buy all products in that set and get discount for 1 subject product
    public class Promotion : MRP
    {
        [Required]
        public Tp Type { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Volume { get; set; } // Discount, QuantityDiscount, SetDiscount - the value of the discount,
        public int? Quantity { get; set; } // QuantityDiscount - the quantity of products to get the discount, QuantityFree - the quantity to get 1 for free, ProductFree - the quantity to get 1 subject for free
        public int? SubjectId { get; set; } // ProductFree, Set - the subject for free, SpecialSetDiscount - the subject to be discounted
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }
        [MaxLength(700)]
        public string DescRu { get; set; }
        [MaxLength(700)]
        public string DescEn { get; set; }
        [MaxLength(700)]
        public string DescTm { get; set; }
        public bool NotInUse { get; set; }
        public ICollection<PromotionProduct> PromotionProducts { get; set; } // Discount, QuantityDiscount, QuantityFree, ProductFree - products in the promotion, Set, SetDiscount, SpecialSetDiscount - set products
    }
}

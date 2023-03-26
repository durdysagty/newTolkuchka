using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class CartOrder
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Image { get; set; }
        public int? DiscountQuantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? QuantityPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RegularPrice { get; set; }
        public int? FreeQuantity { get; set; }
        public int? FreeProductQuantity { get; set; }
        public string FreeProductName { get; set; }
        // property needed to check is any other order do not have same setId, we need only one order got free subject product, not all products in the set
        public int? SetId { get; set; }
        public string SetFreeProductName { get; set; }
        public bool Subjected { get; set; }
    }
}

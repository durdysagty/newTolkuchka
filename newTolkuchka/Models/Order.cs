using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Order : MRP
    {
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal OrderPrice { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int? PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }


        //[MaxLength(100)]
        //public string SerialNumber { get; set; }
        //[Required, Column(TypeName = "decimal(18,2)")]
        //public decimal CostPrice { get; set; }
        //public int Quantity { get; set; }
        //[Required, Column(TypeName = "decimal(18,2)")]
        //public decimal Amount { get; set; }
    }
}

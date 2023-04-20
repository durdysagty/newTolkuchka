using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Purchase : MRP
    {
        [MaxLength(100)]
        public string SerialNumber { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        public int PurchaseInvoiceId { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}

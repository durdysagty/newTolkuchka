using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class PurchaseInvoice : MRP
    {
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal CurrencyRate { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }
}

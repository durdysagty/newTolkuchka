using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Supplier : MRP
    {
        [Required, MaxLength(60)]
        public string Name { get; set; }
        [Required, MaxLength(20)]
        public string PhoneMain { get; set; }
        [MaxLength(20)]
        public string PhoneSecondary { get; set; }
        [Required, MaxLength(100)]
        public string Address { get; set; }

        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
    }
}

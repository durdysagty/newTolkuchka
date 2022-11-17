using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class AdminSupplier
    {
        public int Id { get; set; }
        [Required, MaxLength(60)]
        public string Name { get; set; }
        [Required, MaxLength(20)]
        public string PhoneMain { get; set; }
        public int PurchaseInvoices { get; set; }
    }
}

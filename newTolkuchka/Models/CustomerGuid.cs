using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class CustomerGuid
    {
        [Key]
        public Guid Id { get; set; }
        public int DeniedInvoices { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? BannedDate { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}

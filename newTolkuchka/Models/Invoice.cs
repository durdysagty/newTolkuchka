using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        [Required, MaxLength(200)]
        public string Buyer { get; set; }
        [Required, MaxLength(400)]
        public string InvoiceAddress { get; set; }
        [MaxLength(100)]
        public string InvoiceEmail { get; set; }
        [Required, MaxLength(30)]
        public string InvoicePhone { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryCost { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal CurrencyRate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelivered { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

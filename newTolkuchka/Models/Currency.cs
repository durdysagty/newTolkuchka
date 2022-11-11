using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Currency
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string CodeName { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PriceRate { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal RealRate { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
    }
}

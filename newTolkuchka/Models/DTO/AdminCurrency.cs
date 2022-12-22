using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class AdminCurrency
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string CodeName { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PriceRate { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal RealRate { get; set; }

        //public int Invoices { get; set; }
        //public int PurchaseInvoices { get; set; }
    }
}

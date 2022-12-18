using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class EditProduct
    {
        public int Id { get; set; }
        public string PartNo { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NewPrice { get; set; }
        public bool NotInUse { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsNew { get; set; }
        public bool OnOrder { get; set; }
        public int BrandId { get; set; }
        public int? LineId { get; set; }
        public int? ModelId { get; set; }
    }
}

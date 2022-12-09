using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class AdminProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NewPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public bool NotInUse { get; set; }
    }
}

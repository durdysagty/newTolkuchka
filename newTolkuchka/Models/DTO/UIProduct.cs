using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class UIProduct : MRP
    {
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NewPrice { get; set; }
        public string ImageMain { get; set; }
        public string Recommended { get; set; }
        public string New { get; set; }
        public ICollection<UIPromotion> Promotions { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class UIProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NewPrice { get; set; }
        public string ImageMain { get; set; }
        public string Recommended { get; set; }
        public string New { get; set; }
        // used for colored options of Product (id, price, newPrice)
        public IEnumerable<(int, decimal, decimal)> Others { get; set; }
    }
}

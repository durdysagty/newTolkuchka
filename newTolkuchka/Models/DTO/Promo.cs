using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    // used for Product Page Promotions, Product Page Version of Promotion
    public class Promo
    {
        public int Id { get; set; }
        [Required]
        public Tp Type { get; set; }
        public decimal? Volume { get; set; }
        public int? Quantity { get; set; }
        public Product Subject { get; set; }
        public string Name { get; set; }
        //public string Desc { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}

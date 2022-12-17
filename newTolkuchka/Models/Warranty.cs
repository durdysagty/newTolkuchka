using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Warranty
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }

        public ICollection<Model> Models { get; set; }
        public ICollection<Product> Products { get; set; } // to remove
    }
}

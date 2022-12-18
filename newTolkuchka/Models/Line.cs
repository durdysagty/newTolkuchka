using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Line
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}

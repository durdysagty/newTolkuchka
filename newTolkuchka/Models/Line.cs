using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Line : MRP
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}

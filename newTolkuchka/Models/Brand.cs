using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Brand: MRP
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public bool IsForHome { get; set; }

        public ICollection<Line> Lines { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public bool IsForHome { get; set; }

        public ICollection<Line> Lines { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        // this property is only a hint for the brands of products included into the categories they are in
        // this property is only a hint for the lines of products included into the brands they are in
        public ICollection<Line> Lines { get; set; }
        public ICollection<Model> Models { get; set; }
    }
}

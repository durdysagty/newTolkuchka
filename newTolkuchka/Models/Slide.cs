using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public enum Layout { Main, Left }
    public class Slide
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; }
        [Required, MaxLength(70)]
        public string Link { get; set; }
        public Layout Layout { get; set; }
        public bool NotInUse { get; set; }
    }
}

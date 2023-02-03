using System.ComponentModel.DataAnnotations;
using static newTolkuchka.Services.CultureProvider;

namespace newTolkuchka.Models
{
    public class Heading
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public Culture Language { get; set; }
        public ICollection<HeadingArticle> HeadingArticles { get; set; }
    }
}

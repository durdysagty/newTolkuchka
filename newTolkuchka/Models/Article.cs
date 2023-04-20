using Microsoft.EntityFrameworkCore.Metadata.Internal;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class Article: MRP
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        [Required, MaxLength(ConstantsService.ARTICLEMAXLENGTH)]
        public string Text { get; set; }
        public ICollection<HeadingArticle> HeadingArticles { get; set; }
    }
}

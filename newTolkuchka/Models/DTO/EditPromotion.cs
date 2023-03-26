using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class EditPromotion
    {
        public int Id { get; set; }
        [Required]
        public Tp Type { get; set; }
        public decimal? Volume { get; set; }
        public int? Quantity { get; set; }
        public int? SubjectId { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }
        [MaxLength(700)]
        public string DescRu { get; set; }
        [MaxLength(700)]
        public string DescEn { get; set; }
        [MaxLength(700)]
        public string DescTm { get; set; }
        public bool NotInUse { get; set; }
    }
}

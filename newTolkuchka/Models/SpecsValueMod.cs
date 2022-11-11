using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class SpecsValueMod
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }

        public int SpecsValueId { get; set; }
        public SpecsValue SpecsValue { get; set; }
        public ICollection<ProductSpecsValueMod> ProductSpecsValueMods { get; set; }
    }
}

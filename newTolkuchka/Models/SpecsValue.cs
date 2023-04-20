using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class SpecsValue : MRP
    {
        [Required, MaxLength(300)]
        public string NameRu { get; set; }
        [Required, MaxLength(300)]
        public string NameEn { get; set; }
        [Required, MaxLength(300)]
        public string NameTm { get; set; }

        public int SpecId { get; set; }
        public Spec Spec { get; set; }
        public ICollection<SpecsValueMod> SpecsValueMods { get; set; }
        public ICollection<ProductSpecsValue> ProductSpecsValues { get; set; }
    }
}

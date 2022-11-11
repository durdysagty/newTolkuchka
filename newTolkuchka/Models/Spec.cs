using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Spec
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }
        public bool IsFilter { get; set; }
        public bool IsImaged { get; set; }
        public int Order { get; set; }
        public int? NamingOrder { get; set; }

        public ICollection<SpecsValue> SpecsValues { get; set; }
        public ICollection<ModelSpec> ModelSpecs { get; set; }
    }
}

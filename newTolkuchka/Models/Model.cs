using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Model
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int? LineId { get; set; }
        public Line Line { get; set; }
        public int? WarrantyId { get; set; }
        public Warranty Warranty { get; set; }
        [Required, MaxLength(1500)]
        public string DescRu { get; set; }

        [Required, MaxLength(1500)]
        public string DescEn { get; set; }

        [Required, MaxLength(1500)]
        public string DescTm { get; set; }
        public ICollection<CategoryModelAdLink> CategoryModelAdLinks { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ModelSpec> ModelSpecs { get; set; }
    }
}

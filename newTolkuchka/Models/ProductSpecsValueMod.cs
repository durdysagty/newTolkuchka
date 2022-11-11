namespace newTolkuchka.Models
{
    public class ProductSpecsValueMod
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SpecsValueModId { get; set; }
        public SpecsValueMod SpecsValueMod { get; set; }
    }
}

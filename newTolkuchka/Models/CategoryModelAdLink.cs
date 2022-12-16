namespace newTolkuchka.Models
{
    public class CategoryModelAdLink
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ModelId { get; set; }
        public Model Model { get; set; }
    }
}

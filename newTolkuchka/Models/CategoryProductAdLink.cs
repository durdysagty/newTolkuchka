namespace newTolkuchka.Models // to remove
{
    public class CategoryProductAdLink
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

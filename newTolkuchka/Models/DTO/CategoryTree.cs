namespace newTolkuchka.Models.DTO
{
    public class CategoryTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CategoryTree> List { get; set; }
    }
}

namespace newTolkuchka.Models.DTO
{
    public class AdminCategoryTree: CategoryTree
    {
        public int Level { get; set; }
        public bool HasProduct { get; set; }
        public new IEnumerable<AdminCategoryTree> List { get; set; }
    }
}

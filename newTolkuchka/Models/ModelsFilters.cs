namespace newTolkuchka.Models
{
    public class ModelsFilters<T>
    {
        public IEnumerable<T> Models { get; set; }
        public IEnumerable<string> Filters { get; set; }
        public int LastPage { get; set; }
        public string Pagination { get; set; }
    }
}

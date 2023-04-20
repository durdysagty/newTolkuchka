namespace newTolkuchka.Models.DTO
{
    public class ModelsFilters<T>
    {
        public IEnumerable<T> Models { get; set; }
        public int LastPage { get; set; }
        public string Pagination { get; set; }
    }
}

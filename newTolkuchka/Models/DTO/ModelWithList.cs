namespace newTolkuchka.Models.DTO
{
    public class ModelWithList<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Is { get; set; }

        public ICollection<T> List { get; set; }
    }
}

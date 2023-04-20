namespace newTolkuchka.Models.DTO
{
    public class ModelWithList<T>: MRP
    {
        public string Name { get; set; }
        public bool Is { get; set; }

        public ICollection<T> List { get; set; }
    }
}

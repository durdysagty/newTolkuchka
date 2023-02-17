namespace newTolkuchka.Models.DTO
{
    public class AdminSpec
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpecsValues { get; set; }
        public int Order { get; set; }
        public int? NamingOrder { get; set; }
        public bool IsFilter { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class AdminModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Line { get; set; }
        public int Products { get; set; }
    }
}

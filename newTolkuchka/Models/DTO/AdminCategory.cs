using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class AdminCategory
    {
        public int Id { get; set; }
        public int Padding { get; set; }
        public string Name { get; set; }
        public int Products { get; set; }
    }
}

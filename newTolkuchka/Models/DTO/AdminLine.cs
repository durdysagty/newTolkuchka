using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class AdminLine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int Products { get; set; }
    }
}

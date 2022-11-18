using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{   public class AdminEntry
    {
        public int Id { get; set; }
        public string Employee { get; set; }
        public string Act { get; set; }
        public string Entity { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}

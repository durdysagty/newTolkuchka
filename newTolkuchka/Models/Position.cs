using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Position
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public int Level { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}

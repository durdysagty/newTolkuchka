using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Login { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }
        // if password or position changes hash have to be changed to force authorized user log out
        public string Hash { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }
    }
}

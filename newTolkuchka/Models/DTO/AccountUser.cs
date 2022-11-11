using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class AccountUser
    {
        [MaxLength(100)]
        public string Phone { get; set; }
        public int Pin { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? BirthDay { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
    }
}

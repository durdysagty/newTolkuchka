using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class AdminUserData: AdminUser
    {
        [Column(TypeName = "Date")]
        public DateTime? BirthDay { get; set; }
        public string Address { get; set; }
        public int Wishes { get; set; }
    }
}

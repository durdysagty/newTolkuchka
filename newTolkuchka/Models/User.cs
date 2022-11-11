using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class User: AccountUser
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        [MaxLength(30)]
        public new string Pin { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Wish> Wishes { get; set; }
    }
}

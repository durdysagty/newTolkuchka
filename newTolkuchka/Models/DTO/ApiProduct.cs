using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models
{
    public class ApiProduct
    {
        // used for goozle.com.tm
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Specs { get; set; }
    }
}

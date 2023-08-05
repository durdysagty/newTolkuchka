using System.ComponentModel.DataAnnotations.Schema;

namespace newTolkuchka.Models.DTO
{
    public class UIPromotion : MRP
    {
        public Tp Type { get; set; }
        public decimal? Volume { get; set; }
        public int? Quantity { get; set; }
        public int? SubjectId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}

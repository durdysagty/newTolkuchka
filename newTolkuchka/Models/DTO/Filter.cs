using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class Filter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsImaged { get; set; }
        public IList<FilterValue> FilterValues { get; set; }
    }
}

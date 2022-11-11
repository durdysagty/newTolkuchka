using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    // additional link for categories
    public class CategoryAdLink
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int StepParentId { get; set; }
    }
}

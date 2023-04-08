using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public enum Act { Add, Edit, Delete }
    public enum Entity { Brand, Category, Content, Currency, Employee, Invoice, Line, Model, Order, Position, Product, Purchase, PurchaseInvoice, Slide, Spec, SpecsValue, SpecsValueMod, Supplier, Type, Warranty, Wish, Default, Article, Promotion, User }
    public class Entry
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Employee { get; set; }
        [Required]
        public Act Act { get; set; }
        [Required]
        public Entity Entity { get; set; }
        [Required]
        public int EntityId { get; set; }
        [MaxLength(300)]
        public string EntityName { get; set; }
        [Required]
        public DateTimeOffset DateTime { get; set; }
    }
}

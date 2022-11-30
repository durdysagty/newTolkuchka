namespace newTolkuchka.Models.DTO
{
    public class AdminStoreOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal OrderPrice { get; set; }
        public int? PurchaseId { get; set; }
    }
}

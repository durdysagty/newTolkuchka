namespace newTolkuchka.Models.DTO
{
    public class AdminOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal OrderPrice { get; set; }
        public int Quantity { get; set; }
    }
}

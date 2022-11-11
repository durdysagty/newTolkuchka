namespace newTolkuchka.Models.DTO
{
    public class UserInvoice
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public bool Payment { get; set; }
        public bool Delivery { get; set; }
        public decimal DeliveryCost { get; set; }
        public ICollection<UserOrder> UserOrders { get; set; }
    }
}

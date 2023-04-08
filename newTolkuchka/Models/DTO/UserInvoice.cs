namespace newTolkuchka.Models.DTO
{
    public class UserInvoice
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelivered { get; set; }
        public decimal DeliveryCost { get; set; }
        public ICollection<UserOrder> Orders { get; set; }
    }
}

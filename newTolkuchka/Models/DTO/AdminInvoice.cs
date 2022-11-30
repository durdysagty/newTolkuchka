namespace newTolkuchka.Models.DTO
{
    public class AdminInvoice
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string User { get; set; }
        public string Buyer { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Language { get; set; }
        public string CurrencyCodeName { get; set; }
        public decimal CurrencyRate { get; set; }
        public int Orders { get; set; }
        public decimal DeliveryCost { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelivered { get; set; }
    }
}

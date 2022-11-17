namespace newTolkuchka.Models.DTO
{
    public class AdminPurchaseInvoice
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string SupplierName { get; set; }
        public string CurrencyCodeName { get; set; }
        public decimal CurrencyRate { get; set; }
        public int Purchases { get; set; }
    }
}

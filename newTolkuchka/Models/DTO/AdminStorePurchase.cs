namespace newTolkuchka.Models.DTO
{
    public class AdminStorePurchase
    {
        public int Id { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public decimal PurchasePrice { get; set; }
        public string CurrencyCodeName { get; set; }
    }
}

namespace newTolkuchka.Models.DTO
{
    public class AdminReportOrder
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public DateTimeOffset PaidDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal OrderPrice { get; set; }
        public string OrderCurrency { get; set; }
        public decimal OrderCurrencyRate { get; set; }
        public decimal SoldPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchaseCurrency { get; set; }
        public decimal PurchaseCurrencyRate { get; set; }
        public decimal BoughtPrice { get; set; }
        public decimal NetProfit { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ReportService : ServiceNoFile<Invoice, AdminReoprtOrder>, IReport
    {
        //private readonly IInvoice _invoice;
        //private readonly IProduct _product;
        public ReportService(AppDbContext con, IStringLocalizer<Shared> localizer, IInvoice invoice, IProduct product)
            : base(con, localizer)
        {
            //_invoice = invoice;
            //_product = product;
        }
        //public async Task<IList<AdminReoprtOrder>> CreatePeriodReport(DateTimeOffset start, DateTimeOffset end)
        //{
        //    start = start.Offset > TimeSpan.FromSeconds(0) ? start.UtcDateTime + start.Offset : start.UtcDateTime - start.Offset;
        //    end = end.Offset > TimeSpan.FromSeconds(0) ? end.UtcDateTime + end.Offset : end.UtcDateTime - end.Offset;
        //    DateTimeOffset dateTimeOffset2 = start.UtcDateTime + start.Offset;
        //    IEnumerable<Invoice> invoices = _invoice.GetModels().Where(i => i.IsPaid && i.PaidDate >= start && i.PaidDate <= end).Include(i => i.Currency).Include(i => i.Orders).ThenInclude(o => o.Purchase).ThenInclude(p => p.PurchaseInvoice).ThenInclude(pi => pi.Currency);
        //    List<AdminReoprtOrder> reportOrders = new();
        //    foreach (Invoice invoice in invoices)
        //    {
        //        foreach (Order order in invoice.Orders)
        //        {
        //            decimal soldPrice = Math.Round(order.OrderPrice / invoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
        //            decimal boughtPrice = Math.Round(order.Purchase.PurchasePrice / order.Purchase.PurchaseInvoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
        //            AdminReoprtOrder reoprtOrder = new()
        //            {
        //                Id= order.Id,
        //                InvoiceId= invoice.Id,
        //                InvoiceDate = invoice.Date,
        //                PaidDate = invoice.PaidDate.Value,
        //                ProductId= order.ProductId,
        //                ProductName = IProduct.GetProductName(await _product.GetFullProductAsync(order.ProductId)),
        //                OrderPrice = order.OrderPrice,
        //                OrderCurrency = invoice.Currency.CodeName,
        //                OrderCurrencyRate = invoice.CurrencyRate,
        //                SoldPrice = soldPrice,
        //                PurchasePrice = order.Purchase.PurchasePrice,
        //                PurchaseCurrency = order.Purchase.PurchaseInvoice.Currency.CodeName,
        //                PurchaseCurrencyRate = order.Purchase.PurchaseInvoice.CurrencyRate,
        //                BoughtPrice = boughtPrice,
        //                NetProfit = soldPrice - boughtPrice
        //            };
        //            reportOrders.Add(reoprtOrder);
        //        }
        //    }
        //    return reportOrders;
        //}
    }
}

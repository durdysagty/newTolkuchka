using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PurchaseInvoiceService : ServiceNoFile<PurchaseInvoice>, IPurchaseInvoice
    {
        public PurchaseInvoiceService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }

        public IEnumerable<AdminPurchaseInvoice> GetAdminPurchaseInvoices()
        {
            IEnumerable<AdminPurchaseInvoice> purchaseInvoices = GetModels().Select(x => new AdminPurchaseInvoice
            {
                Id = x.Id,
                Date = x.Date,
                SupplierName = x.Supplier.Name,
                CurrencyCodeName = x.Currency.CodeName,
                CurrencyRate = x.CurrencyRate,
                Purchases = x.Purchases.Count
            }).OrderByDescending(x => x.Id);
            return purchaseInvoices;
        }
    }
}
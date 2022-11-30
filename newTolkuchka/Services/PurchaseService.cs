using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PurchaseService : ServiceNoFile<Purchase>, IPurchase
    {
        private readonly IProduct _product;
        public PurchaseService(AppDbContext con, IStringLocalizer<Shared> localizer, IProduct product) : base(con, localizer)
        {
            _product = product;
        }

        public async Task AddPurchasesAsync(int purchaseInvoiceId, IList<AdminPurchase> adminPurchases)
        {
            foreach (AdminPurchase ap in adminPurchases)
            {
                for (int i = 0; i < ap.Quantity; i++)
                {
                    Purchase purchase = new()
                    {
                        SerialNumber = ap.SerialNumbers[i],
                        PurchasePrice = ap.PurchasePrice,
                        PurchaseInvoiceId = purchaseInvoiceId,
                        ProductId = ap.ProductId,
                    };
                    await AddModelAsync(purchase, false);
                }
            }
        }
        public async Task<IEnumerable<AdminPurchase>> GetAdminPurchasesByPurchaseInvoiceId(int id)
        {
            IEnumerable<Purchase> purchases = GetPurchasesByPurchaseInvoiceId(id);
            IList<AdminPurchase> adminPurchases = new List<AdminPurchase>();
            foreach (Purchase p in purchases)
            {
                AdminPurchase adminPurchase = adminPurchases.FirstOrDefault(ap => ap.ProductId == p.ProductId);
                if (adminPurchase == null)
                {
                    adminPurchase = new()
                    {
                        ProductId = p.ProductId,
                        Name = IProduct.GetProductName(await _product.GetFullProductAsync(p.ProductId)),
                        SerialNumbers = new List<string>()
                        {
                            p.SerialNumber
                        },
                        PurchasePrice = p.PurchasePrice,
                        Quantity = 1
                    };
                    adminPurchases.Add(adminPurchase);
                }
                else
                {
                    adminPurchase.Quantity++;
                    adminPurchase.SerialNumbers.Add(p.SerialNumber);
                }
            }
            return adminPurchases;
        }

        public IEnumerable<AdminStorePurchase> GetAdminStorePurchases(int[] ids, int[] usedIds)
        {
            IEnumerable<AdminStorePurchase> purchases = GetModels().Where(p => (ids.Contains(p.ProductId) && p.Order == null) || usedIds.Contains(p.Id)).Select(p => new AdminStorePurchase
            {
                Id = p.Id,
                ProductId = p.ProductId,
                SerialNumber = p.SerialNumber,
                PurchasePrice = p.PurchasePrice,
                PurchaseInvoiceId = p.PurchaseInvoice.Id,
                CurrencyCodeName = p.PurchaseInvoice.Currency.CodeName
            });
            return purchases;
        }

        public async Task<Result> RemovePurchaseInvoicePurchases(int id)
        {
            IEnumerable<Purchase> purchases = GetPurchasesByPurchaseInvoiceId(id);
            foreach (Purchase p in purchases)
            {
                 Result result = await DeleteModelAsync(p.Id, p);
                if (result == Result.Fail)
                    return result;
            }
            return Result.Success;
        }

        private IEnumerable<Purchase> GetPurchasesByPurchaseInvoiceId(int id)
        {
            return GetModels().Where(p => p.PurchaseInvoiceId == id);
        }
    }
}

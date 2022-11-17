using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPurchase : IActionNoFile<Purchase>
    {
        Task AddPurchasesAsync(int purchaseInvoiceId, IList<AdminPurchase> adminPurchases);
        Task<IEnumerable<AdminPurchase>> GetPurchaseInvoicePurchases(int id);
        Task<Result> RemovePurchaseInvoicePurchases(int id);
    }
}

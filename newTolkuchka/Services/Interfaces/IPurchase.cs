using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPurchase : IActionNoFile<Purchase, AdminPurchase>
    {
        Task AddPurchasesAsync(int purchaseInvoiceId, IList<AdminPurchase> adminPurchases);
        Task<IEnumerable<AdminPurchase>> GetAdminPurchasesByPurchaseInvoiceId(int id);
        IEnumerable<AdminStorePurchase> GetAdminStorePurchases(int[] ids, int[] usedIds);
        Task<Result> RemovePurchaseInvoicePurchases(int id);
    }
}

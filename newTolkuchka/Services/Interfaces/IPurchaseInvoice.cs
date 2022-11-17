using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPurchaseInvoice : IActionNoFile<PurchaseInvoice>
    {
        IEnumerable<AdminPurchaseInvoice> GetAdminPurchaseInvoices();
    }
}

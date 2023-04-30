using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IInvoice : IActionNoFile<Invoice, AdminInvoice>
    {
        Task CreateInvoice(int? userId, CartOrder[] cartOrders, DeliveryData deliveryData, Guid customerGuidId);
        Task<IEnumerable<UserInvoice>> GetUserInvoicesAsync(int userId);
        IEnumerable<AdminInvoice> GetAdminInvoices();
    }
}

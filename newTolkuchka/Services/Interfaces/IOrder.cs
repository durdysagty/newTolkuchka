using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IOrder : IActionNoFile<Order>
    {
        void CreateCartOrders(IList<CartOrder> orders);
        IEnumerable<Order> GetOrdersByInvoiceId(int id);
        Task<IEnumerable<AdminOrder>> GetAdminOrdersByInvoiceIdAsync(int id);
        IEnumerable<AdminStoreOrder> GetAdminStoreOrdersByInvoiceIdAsync(int id);
        Task CorrectOrdersAsync(int invoiceId, IList<AdminOrder> adminOrders);
    }
}

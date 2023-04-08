using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IOrder : IActionNoFile<Order, AdminOrderExtended>
    {
        void CreateCartOrders(IList<CartOrder> orders);
        Task<decimal> CreateOrders(int invoiceId, IList<CartOrder> cartOrders);
        IEnumerable<Order> GetOrdersByInvoiceId(int id);
        Task<IEnumerable<AdminOrderExtended>> GetAdminOrdersByInvoiceIdAsync(int id);
        IEnumerable<AdminStoreOrder> GetAdminStoreOrdersByInvoiceIdAsync(int id);
        Task CorrectOrdersAsync(int invoiceId, IList<AdminOrderExtended> adminOrders);
    }
}

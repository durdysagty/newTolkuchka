using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IOrder : IActionNoFile<Order>
    {
        void CreateCartOrders(IList<CartOrder> orders);
    }
}

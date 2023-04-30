using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ICustomerGuid : IActionNoFile<CustomerGuid, CustomerGuid>
    {
        Task AddModelAsync(CustomerGuid customerGuid);
    }
}

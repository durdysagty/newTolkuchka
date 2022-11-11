using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IInvoice : IActionNoFile<Invoice>
    {
        Task<IEnumerable<UserInvoice>> GetUserInvoicesAsync(int userId);
    }
}

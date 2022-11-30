using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IInvoice : IActionNoFile<Invoice>
    {
        AdminInvoice GetAdminInvoice(int id);
        Task<IEnumerable<UserInvoice>> GetUserInvoicesAsync(int userId);
        IEnumerable<AdminInvoice> GetAdminInvoices();
    }
}

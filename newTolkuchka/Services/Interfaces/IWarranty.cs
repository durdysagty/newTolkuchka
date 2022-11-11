using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IWarranty : IActionNoFile<Warranty>
    {
        IEnumerable<AdminWarranty> GetAdminWarranties();
    }
}

using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISupplier : IActionNoFile<Supplier>
    {
        IEnumerable<AdminSupplier> GetAdminSuppliers();
    }
}

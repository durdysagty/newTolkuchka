using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IBrand : IActionFormFile<Brand>
    {
        IEnumerable<AdminBrand> GetAdminBrands();
    }
}

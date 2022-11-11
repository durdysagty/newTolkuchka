using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ILine : IActionNoFile<Line>
    {
        IEnumerable<AdminLine> GetAdminLines(int? brandId);
    }
}

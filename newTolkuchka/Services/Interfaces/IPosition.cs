using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPosition: IActionNoFile<Position>
    {
        IEnumerable<AdminPosition> GetAdminPositions();
        void EditPosition(Position position);
    }
}

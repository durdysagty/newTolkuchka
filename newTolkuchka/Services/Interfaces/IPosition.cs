using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPosition: IActionNoFile<Position, AdminPosition>
    {
        void EditPosition(Position position);
    }
}

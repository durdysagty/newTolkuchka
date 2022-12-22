using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEntry : IActionNoFile<Entry, AdminEntry>
    {
        Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName);
    }
}

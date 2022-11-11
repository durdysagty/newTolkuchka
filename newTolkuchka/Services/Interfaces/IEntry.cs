using newTolkuchka.Models;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEntry
    {
        IQueryable<Entry> GetEntries();
        Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName);
    }
}

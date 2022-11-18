using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEntry
    {
        IQueryable<AdminEntry> GetEntries(int[] employeeId, int page, int pp, out int lastPage, out string pagination);
        Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName);
    }
}

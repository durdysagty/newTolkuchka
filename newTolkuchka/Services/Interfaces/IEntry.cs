using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEntry : IActionNoFile<Entry, AdminEntry>
    {
        Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName, bool? siteMapToAdd = null, CultureProvider.Culture? culture = null);
        //void CorrectSiteMap(string entity, int entityId, bool add);
        Task CorrectSiteMap(string entity, IList<(int, bool)> entities, CultureProvider.Culture? culture = null);
    }
}

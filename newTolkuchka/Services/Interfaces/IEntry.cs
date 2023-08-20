using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using static newTolkuchka.Services.CultureProvider;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEntry : IActionNoFile<Entry, AdminEntry>
    {
        Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName, bool? siteMapToAdd = null, CultureProvider.Culture? culture = null);
        //void CorrectSiteMap(string entity, int entityId, bool add);
        void CacheForSiteMap(string entity, IList<(int, bool, Culture?)> entities);
    }
}

using Microsoft.AspNetCore.Authorization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class EntryController : AbstractController<Entry, AdminEntry, IEntry>
    {
        public EntryController(IEntry entry, ICacheClean cacheClean) : base(entry, Entity.Default, entry, cacheClean)
        {
        }
    }
}
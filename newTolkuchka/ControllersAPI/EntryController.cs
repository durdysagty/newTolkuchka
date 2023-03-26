using Microsoft.AspNetCore.Authorization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class EntryController : AbstractController<Entry, AdminEntry, IEntry>
    {
        public EntryController(IEntry entry) : base(entry, Entity.Default, entry)
        {
        }
        //[HttpGet]
        //public ModelsFilters<AdminEntry> Get([FromQuery] int page = 0, [FromQuery] int pp = 100)
        //{
        //    IEnumerable<AdminEntry> entries = _entry.GetEntries(page, pp, out int lastPage, out string pagination);
        //    return new ModelsFilters<AdminEntry>
        //    {
        //        Models = entries,
        //        LastPage = lastPage,
        //        Pagination = pagination
        //    };
        //}
    }
}
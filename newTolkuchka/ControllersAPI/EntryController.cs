using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [ApiController, Route("api/[controller]")]
    [Authorize(Policy = "Level3")]
    public class EntryController
    {
        private readonly IEntry _entry;
        public EntryController(IEntry entry)
        {
            _entry = entry;
        }
        [HttpGet]
        public ModelsFilters<AdminEntry> Get([FromQuery] int[] employee, [FromQuery] int page = 0, [FromQuery] int pp = 100)
        {
            IEnumerable<AdminEntry> entries = _entry.GetEntries(employee, page, pp, out int lastPage, out string pagination);
            return new ModelsFilters<AdminEntry>
            {
                Filters = new string[1] { nameof(employee) },
                Models = entries,
                LastPage = lastPage,
                Pagination = pagination
            };
        }
    }
}
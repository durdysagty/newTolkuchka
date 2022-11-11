using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EntryService : IEntry
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _con;
        public EntryService(IHttpContextAccessor contextAccessor, AppDbContext con)
        {
            _contextAccessor = contextAccessor;
            _con = con;
        }

        public async Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName)
        {
            Entry entry = new()
            {
                Employee = IContext.GetAthorizedUserId(_contextAccessor.HttpContext),
                Act = act,
                Entity = entity,
                EntityId = entityId,
                DateTime = DateTimeOffset.UtcNow,
                EntityName = entityName
            };
            await _con.Entries.AddAsync(entry);
            await _con.SaveChangesAsync();
        }

        // to be finished
        public IQueryable<Entry> GetEntries()
        {
            return _con.Entries.OrderByDescending(x => x.Id);
        }
    }
}

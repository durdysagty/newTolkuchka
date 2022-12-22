using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EntryService : ServiceNoFile<Entry, AdminEntry>, IEntry
    {
        private readonly IHttpContextAccessor _contextAccessor;
        //private readonly IEmployee _employee;
        public EntryService(IHttpContextAccessor contextAccessor, AppDbContext con,/* IEmployee employee,*/  IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName)
        {
            Entry entry = new()
            {
                Employee = IContext.GetAthorizedUserId(_contextAccessor.HttpContext),
                Act = act,
                Entity = entity,
                EntityId = entityId,
                DateTime = DateTimeOffset.UtcNow.ToUniversalTime(),
                EntityName = entityName
            };
            await AddModelAsync(entry, true);
        }
    }
}

using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EntryService : BaseService, IEntry
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _con;
        private readonly IEmployee _employee;
        public EntryService(IHttpContextAccessor contextAccessor, AppDbContext con, IEmployee employee,  IStringLocalizer<Shared> localizer) : base(localizer)
        {
            _contextAccessor = contextAccessor;
            _con = con;
            _employee = employee;
        }
        public IQueryable<AdminEntry> GetEntries(int[] employeeId, int page, int pp, out int lastPage, out string pagination)
        {
            IQueryable<Entry> entries = _con.Entries.OrderByDescending(x => x.Id);
            if (employeeId.Any())
            {
                IQueryable<string> names = _employee.GetEmployeeNames(employeeId);
                entries = entries.Where(e => names.Any(n => n == e.Employee));
            }
            int toSkip = page * pp;
            IQueryable<AdminEntry> adminEntries = entries.Skip(toSkip).Take(pp).Select(e => new AdminEntry
            {
                Id = e.Id,
                Employee = e.Employee,
                Act = _localizer[e.Act.ToString().ToLower()],
                Entity = _localizer[e.Entity.ToString().ToLower()],
                EntityId = e.EntityId,
                EntityName = e.EntityName,
                Date = e.DateTime,
            });
            pagination = GetPagination(pp, entries.Count(), adminEntries.Count(), toSkip, out int lp);
            lastPage = lp;
            return adminEntries;
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
            _ = await _con.Entries.AddAsync(entry);
            _ = await _con.SaveChangesAsync();
        }
    }
}

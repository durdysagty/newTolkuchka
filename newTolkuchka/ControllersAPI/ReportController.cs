using Microsoft.AspNetCore.Authorization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class ReportController : AbstractController<Invoice, AdminReportOrder, IReport>
    {
        public ReportController(IEntry entry, IReport report, ICacheClean cacheClean) : base(entry, Entity.Default, report, cacheClean)
        {
        }
    }
}

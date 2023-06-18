using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ReportService : ServiceNoFile<Invoice, AdminReportOrder>, IReport
    {
        public ReportService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, memoryCache, localizer, cacheClean)
        {
        }
    }
}

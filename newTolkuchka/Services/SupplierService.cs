using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SupplierService : ServiceNoFile<Supplier, AdminSupplier>, ISupplier
    {
        public SupplierService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, memoryCache, localizer, cacheClean)
        {
        }
    }
}
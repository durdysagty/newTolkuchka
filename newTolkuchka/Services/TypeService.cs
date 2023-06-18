using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Type = newTolkuchka.Models.Type;

namespace newTolkuchka.Services
{
    public class TypeService : ServiceNoFile<Type, AdminType>, IType
    {
        public TypeService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, memoryCache, localizer, cacheClean)
        {
        }
    }
}
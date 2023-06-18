using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecsValueService : ServiceFormFile<SpecsValue, AdminSpecsValue>, ISpecsValue
    {
        private const int IMAGESMAX = 1;
        public SpecsValueService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, IPath path, ICacheClean cacheClean, IImage image) : base(con, memoryCache, localizer, path, cacheClean, image, IMAGESMAX)
        {
        }
    }
}
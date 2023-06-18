using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class BrandService : ServiceFormFile<Brand, AdminBrand>, IBrand
    {
        //private const int IMAGESMAX = 1;
        public BrandService(AppDbContext con, IMemoryCache memoryCache, IPath path, ICacheClean cacheClean, IImage image, IStringLocalizer<Shared> localizer) : base(con, memoryCache, localizer, path, cacheClean, image, ConstantsService.UMAXIMAGE)
        {
        }
    }
}

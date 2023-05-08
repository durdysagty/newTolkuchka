using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeAppController : ControllerBase
    {
        private readonly IBrand _brand;
        private readonly IMemoryCache _memoryCache;

        public HomeAppController(IBrand brand, IMemoryCache memoryCache)
        {
            _brand = brand;
            _memoryCache = memoryCache;
        }

        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<JsonResult> Items()
        {
            IList<Brand> brands = await _memoryCache.GetOrCreate(ConstantsService.HOMEAPPBRANDS, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IList<Brand> brands = await _brand.GetModels().Where(b => b.IsForHome).ToListAsync();
                return brands;
            });
            return new JsonResult(new
            {
                brands
            });
        }
    }
}

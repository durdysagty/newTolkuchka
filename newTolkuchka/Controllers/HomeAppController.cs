using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using F = System.IO.File;

namespace newTolkuchka.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeAppController : ControllerBase
    {
        private readonly IBrand _brand;
        private readonly ICategory _category;
        //private readonly IPath _path;
        private readonly IMemoryCache _memoryCache;

        public HomeAppController(IBrand brand, ICategory category,  IMemoryCache memoryCache)
        {
            _brand = brand;
            _category = category;
            //_path = path;
            _memoryCache = memoryCache;
        }

        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<JsonResult> Items()
        {
            IList<Category> categories = await _memoryCache.GetOrCreate(ConstantsService.HOMEAPPCATEGORIES, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IList<Category> indexCats = await _category.GetIndexCategories().ToListAsync();
                return indexCats;
            });
            IList<Brand> brands = await _memoryCache.GetOrCreate(ConstantsService.HOMEAPPBRANDS, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IList<Brand> brands = await _brand.GetModels().Where(b => b.IsForHome).ToListAsync();
                return brands;
            });
            return new JsonResult(new
            {
                categories,
                brands
            });
        }

        //[HttpGet("{id}")]
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        //public async Task<ContentResult> GetSvg(int id)
        //{
        //    string svg = await F.ReadAllTextAsync($"{_path.GetSVGFolder()}/category/{id}.svg");
        //    return base.Content($"<html style=\"width: fit-content; height: fit-content; margin: 0\"><head><meta name\"viewport\" content=\"width=fit-content, initial-scale=1.0\" ></head><body style=\"width: fit-content; height: fit-content; margin: 0\">{svg}</body></html>", "text/html");
        //}
    }
}

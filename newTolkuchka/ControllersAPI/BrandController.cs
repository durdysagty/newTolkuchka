using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class BrandController : AbstractController<Brand, AdminBrand, IBrand>
    {
        private const int WIDTH = 180;
        private const int HEIGHT = 60;
        public BrandController(IEntry entry, IBrand brand, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Brand, brand, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Brand> Get(int id)
        {
            Brand brand = await _service.GetModelAsync(id);
            return brand;
        }
        //[HttpGet]
        //public IEnumerable<AdminBrand> Get()
        //{
        //    IEnumerable<AdminBrand> brands = _service.GetAdminBrands();
        //    return brands;
        //}
        [HttpPost]
        public async Task<Result> Post([FromForm] Brand brand, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(brand, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(brand, images, WIDTH, HEIGHT);
            await AddActAsync(brand.Id, brand.Name);
            _cacheClean.CleanBrands();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Brand brand, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(brand, _service.GetModels().Where(x => x.Id != brand.Id));
            if (isExist)
                return Result.Already;
            await _service.EditModelAsync(brand, images, WIDTH, HEIGHT);
            await EditActAsync(brand.Id, brand.Name);
            _cacheClean.CleanBrands();
            _cacheClean.CleanProductPage();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Brand brand = await _service.GetModelAsync(id);
            if (brand == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(brand.Id, brand);
            if (result == Result.Success)
                await DeleteActAsync(id, brand.Name);
            _cacheClean.CleanBrands();
            return result;
        }
    }
}
﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IBrand _brand;
        private readonly ICacheClean _cacheClean;
        public BrandController(IEntry entry, IBrand brand, ICacheClean cacheClean) : base(entry, Entity.Brand, brand)
        {
            _brand = brand;
            _cacheClean = cacheClean;
        }

        [HttpGet("{id}")]
        public async Task<Brand> Get(int id)
        {
            Brand brand = await _brand.GetModelAsync(id);
            return brand;
        }
        //[HttpGet]
        //public IEnumerable<AdminBrand> Get()
        //{
        //    IEnumerable<AdminBrand> brands = _brand.GetAdminBrands();
        //    return brands;
        //}
        [HttpPost]
        public async Task<Result> Post([FromForm] Brand brand, [FromForm] IFormFile[] images)
        {
            bool isExist = _brand.IsExist(brand, _brand.GetModels());
            if (isExist)
                return Result.Already;
            await _brand.AddModelAsync(brand, images, WIDTH, HEIGHT);
            await AddActAsync(brand.Id, brand.Name);
            _cacheClean.CleanBrands();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Brand brand, [FromForm] IFormFile[] images)
        {
            bool isExist = _brand.IsExist(brand, _brand.GetModels().Where(x => x.Id != brand.Id));
            if (isExist)
                return Result.Already;
            await _brand.EditModelAsync(brand, images, WIDTH, HEIGHT);
            await EditActAsync(brand.Id, brand.Name);
            _cacheClean.CleanBrands();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Brand brand = await _brand.GetModelAsync(id);
            if (brand == null)
                return Result.Fail;
            Result result = await _brand.DeleteModelAsync(brand.Id, brand);
            if (result == Result.Success)
                await DeleteActAsync(id, brand.Name);
            _cacheClean.CleanBrands();
            return result;
        }
    }
}
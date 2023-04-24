using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class ProductController : AbstractController<Product, AdminProduct, IProduct>
    {
        private const int WIDTH = 600;
        private const int HEIGHT = 600;
        private const int DIVIDER = 3;
        private readonly IProduct _product;
        private readonly ICacheClean _cacheClean;
        public ProductController(IEntry entry, IProduct product, ICacheClean cacheClean) : base(entry, Entity.Product, product)
        {
            _product = product;
            _cacheClean = cacheClean;
        }

        [HttpGet("{id}")]
        public EditProduct Get(int id)
        {
            EditProduct product = _product.GetEditProduct(id);
            return product;
        }

        [HttpGet("specvalues/{id}")]
        public async Task<string[]> GetSpecValues(int id)
        {
            return await _product.GetSpecValuesAsync(id);
        }
        [HttpGet("specvaluemods/{id}")]
        public async Task<object[]> GetSpecValueMods(int id)
        {
            return await _product.GetSpecValueModsAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Product product, [FromForm] IFormFile[] images, [FromForm] IList<int> specsValues, [FromForm] IList<int> specsValueMods)
        {
            bool isEqual = await _product.CheckProductSpecValues((int)product.ModelId, specsValues, specsValueMods);
            if (isEqual)
                return Result.Already;
            await _product.AddModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            if (specsValues.Any())
                await _product.AddProductSpecValuesAsync(product.Id, specsValues);
            if (specsValueMods.Any())
                await _product.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _product.SaveChangesAsync();
            product = await _product.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await AddActAsync(product.Id, IProduct.GetProductNameCounted(product));
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Product product, [FromForm] IFormFile[] images, [FromForm] IList<int> specsValues, [FromForm] IList<int> specsValueMods)
        {
            bool isEqual = await _product.CheckProductSpecValues((int)product.ModelId, specsValues, specsValueMods, product.Id);
            if (isEqual)
                return Result.Already;
            await _product.EditModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            await _product.AddProductSpecValuesAsync(product.Id, specsValues);
            await _product.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _product.SaveChangesAsync();
            product = await _product.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await EditActAsync(product.Id, IProduct.GetProductNameCounted(product), !product.NotInUse);
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Product product = await _product.GetModelAsync(id);
            if (product == null)
                return Result.Fail;
            Result result = await _product.DeleteModelAsync(product.Id, product, true);
            if (result == Result.Success)
            {
                product = await _product.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
                await DeleteActAsync(id, IProduct.GetProductNameCounted(product));
            }
            return result;
        }
        [HttpPost("changeprice")]
        public async Task<Result> Post([FromForm] decimal price, [FromForm] int[] priceIds, [FromForm] int[] newPriceIds)
        {
            IList<Product> products = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, priceIds.Concat(newPriceIds).Distinct().ToList() } }).ToArrayAsync();
            foreach (Product product in products)
            {
                if (priceIds.Contains(product.Id))
                    product.Price = price;
                if (newPriceIds.Contains(product.Id))
                    product.NewPrice = price == 0 ? null : price;
                await EditActAsync(product.Id, IProduct.GetProductNameCounted(product));
            }
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            return Result.Success;
        }
    }
}
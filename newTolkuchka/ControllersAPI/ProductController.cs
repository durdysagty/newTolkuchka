using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public ProductController(IEntry entry, IProduct product, ICacheClean cacheClean) : base(entry, Entity.Product, product, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public EditProduct Get(int id)
        {
            EditProduct product = _service.GetEditProduct(id);
            return product;
        }

        [HttpGet("specvalues/{id}")]
        public async Task<string[]> GetSpecValues(int id)
        {
            return await _service.GetSpecValuesAsync(id);
        }
        [HttpGet("specvaluemods/{id}")]
        public async Task<object[]> GetSpecValueMods(int id)
        {
            return await _service.GetSpecValueModsAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Product product, [FromForm] IFormFile[] images, [FromForm] IList<int> specsValues, [FromForm] IList<int> specsValueMods)
        {
            bool isEqual = await _service.CheckProductSpecValues((int)product.ModelId, specsValues, specsValueMods);
            if (isEqual)
                return Result.Already;
            await _service.AddModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            if (specsValues.Any())
                await _service.AddProductSpecValuesAsync(product.Id, specsValues);
            if (specsValueMods.Any())
                await _service.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _service.SaveChangesAsync();
            product = await _service.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await AddActAsync(product.Id, IProduct.GetProductNameCounted(product));
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Product product, [FromForm] IFormFile[] images, [FromForm] IList<int> specsValues, [FromForm] IList<int> specsValueMods)
        {
            bool isEqual = await _service.CheckProductSpecValues((int)product.ModelId, specsValues, specsValueMods, product.Id);
            if (isEqual)
                return Result.Already;
            await _service.EditModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            await _service.AddProductSpecValuesAsync(product.Id, specsValues);
            await _service.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _service.SaveChangesAsync();
            product = await _service.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await EditActAsync(product.Id, IProduct.GetProductNameCounted(product), !product.NotInUse);
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            _cacheClean.CleanProductPage(product.Id);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Product product = await _service.GetModelAsync(id);
            if (product == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(product.Id, product, true);
            if (result == Result.Success)
            {
                product = await _service.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
                await DeleteActAsync(id, IProduct.GetProductNameCounted(product));
            }
            _cacheClean.CleanProductPage(product.Id);
            return result;
        }
        [HttpPost("changeprice")]
        public async Task<Result> Post([FromForm] decimal price, [FromForm] int[] priceIds, [FromForm] int[] newPriceIds)
        {
            IList<Product> products = await _service.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, priceIds.Concat(newPriceIds).Distinct().ToList() } }).ToArrayAsync();
            foreach (Product product in products)
            {
                if (priceIds.Contains(product.Id))
                    product.Price = price;
                if (newPriceIds.Contains(product.Id))
                    product.NewPrice = price == 0 ? null : price;
                _cacheClean.CleanProductPage(product.Id);
                await EditActAsync(product.Id, IProduct.GetProductNameCounted(product));
            }
            _cacheClean.CleanIndexItems();
            _cacheClean.CleanAllModeledProducts();
            return Result.Success;
        }
    }
}
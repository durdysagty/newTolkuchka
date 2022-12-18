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
    public class ProductController : AbstractController<Product, IProduct>
    {
        private const int WIDTH = 600;
        private const int HEIGHT = 600;
        private const int DIVIDER = 3;
        private readonly IProduct _product;
        public ProductController(IEntry entry, IProduct product) : base(entry, Entity.Product, product)
        {
            _product = product;
        }

        [HttpGet("{id}")]
        public EditProduct Get(int id)
        {
            EditProduct product = _product.GetEditProduct(id);
            return product;
        }
        [HttpGet]
        public ModelsFilters<AdminProduct> Get([FromQuery] int[] category, [FromQuery] int[] brand, [FromQuery] int? line, [FromQuery] int? model, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            IEnumerable<AdminProduct> products = _product.GetAdminProducts(category.Any() ? category : null, brand.Any() ? brand : null, line, model, page, pp, out int lastPage, out string pagination);
            return new ModelsFilters<AdminProduct>
            {
                //Filters = new string[4] { nameof(category), nameof(brand), $"{nameof(brand)}Id {nameof(line)}", $"{nameof(line)}Id {nameof(model)}" }.OrderBy(c => c),
                Filters = new string[4] { nameof(category), nameof(brand), $"{nameof(brand)}Id {nameof(line)}", $"{nameof(line)}Id {nameof(model)}" }.OrderBy(c => c),
                Models = products,
                LastPage = lastPage,
                Pagination = pagination
            };
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
            bool isEqual = await _product.CheckProductSpecValues((int)product.ModelId, specsValues);
            if (isEqual)
                return Result.Already;
            await _product.AddModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            if (specsValues.Any())
                await _product.AddProductSpecValuesAsync(product.Id, specsValues);
            if (specsValueMods.Any())
                await _product.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _product.SaveChangesAsync();
            product = await _product.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await AddActAsync(product.Id, IProduct.GetProductName(product));
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Product product, [FromForm] IFormFile[] images, [FromForm] IList<int> specsValues, [FromForm] IList<int> specsValueMods)
        {
            bool isEqual = await _product.CheckProductSpecValues((int)product.ModelId, specsValues, product.Id);
            if (isEqual)
                return Result.Already;
            await _product.EditModelAsync(product, images, WIDTH, HEIGHT, DIVIDER);
            await _product.AddProductSpecValuesAsync(product.Id, specsValues);
            await _product.AddProductSpecValueModsAsync(product.Id, specsValueMods);
            await _product.SaveChangesAsync();
            product = await _product.GetFullProductAsNoTrackingWithIdentityResolutionAsync(product.Id);
            await EditActAsync(product.Id, IProduct.GetProductName(product));
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
                await DeleteActAsync(id, IProduct.GetProductName(product));
            }
            return result;
        }
        [HttpPost("changeprice")]
        public async Task<Result> Post([FromForm] decimal price, [FromForm] int[] priceIds, [FromForm] int[] newPriceIds)
        {
            IList<Product> products = await _product.GetFullProducts(null, null, null, null, null, priceIds.Concat(newPriceIds).Distinct().ToList()).ToListAsync();
            foreach (Product product in products)
            {
                if (priceIds.Contains(product.Id))
                    product.Price = price;
                if (newPriceIds.Contains(product.Id))
                    product.NewPrice = price == 0 ? null : price;
                await EditActAsync(product.Id, IProduct.GetProductName(product));
            }
            return Result.Success;
        }
    }
}
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
    public class CategoryController : AbstractController<Category, AdminCategory, ICategory>
    {
        private const int WIDTH = 450;
        private const int HEIGHT = 225;
        private readonly IProduct _product;
        public CategoryController(IEntry entry, ICategory category, ICacheClean cacheClean, IMemoryCache memoryCache, IProduct product) : base(entry, Entity.Category, category, memoryCache, cacheClean)
        {
            _product = product;
        }

        [HttpGet("{id}")]
        public async Task<Category> Get(int id)
        {
            Category category = await _service.GetModelAsync(id);
            return category;
        }
        [HttpGet("hasproduct/{id}")]
        public async Task<bool> HasProduct(int id)
        {
            return await _service.HasProduct(id);
        }
        [HttpGet("tree")]
        public async Task<IEnumerable<AdminCategoryTree>> GetTree()
        {
            IEnumerable<AdminCategoryTree> categories = await _service.GetAdminCategoryTree();
            return categories;
        }
        [HttpGet("adlinks/{id}")]
        public async Task<string[]> GetAdLinks(int id)
        {
            return await _service.GetAdLinksAsync(id);
        }
        [HttpGet("modeladlinks/{id}")]
        public async Task<string[]> GetModelAdLinks(int id)
        {
            return await _service.GetModelAdLinksAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Category category, [FromForm] int[] adLinks, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(category, _service.GetCategoriesByParentId(category.ParentId));
            if (isExist)
                return Result.Already;
            if (category.IsForHome)
            {
                bool isImaged = _service.IsCategoryImaged(images);
                if (!isImaged)
                    return Result.NoImage;
            }
            Category parent = await _service.GetModelAsync(category.ParentId);
            if (parent != null)
            {
                if (parent.NotInUse)
                {
                    category.NotInUse = true;
                }
            }
            await _service.AddModelAsync(category, images, WIDTH, HEIGHT);
            await _service.AddCategoryAdLinksAsync(category.Id, adLinks);
            await AddActAsync(category.Id, category.NameRu);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Category category, [FromForm] int[] adLinks, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(category, _service.GetCategoriesByParentId(category.ParentId).Where(x => x.Id != category.Id));
            if (isExist)
                return Result.Already;
            if (category.IsForHome)
            {
                bool isImaged = _service.IsCategoryImaged(images, category.Id);
                if (!isImaged)
                    return Result.NoImage;
            }
            IList<(int, bool, CultureProvider.Culture?)> categoriesToSiteMap = new List<(int, bool, CultureProvider.Culture?)>();
            IList<(int, bool, CultureProvider.Culture?)> productsToSiteMap = new List<(int, bool, CultureProvider.Culture?)>();
            async Task AllNotInUse(IEnumerable<Category> categories)
            {
                if (categories.Any())
                    foreach (Category category in categories)
                    {
                        category.NotInUse = true;
                        categoriesToSiteMap.Add((category.Id, !category.NotInUse, null));
                        if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { category.Id } } }).Any())
                            CorrectProductsSiteMap(category.Id, category.NotInUse);
                        await AllNotInUse(_service.GetModels().Where(c => c.ParentId == category.Id));
                    }
            }
            void CorrectProductsSiteMap(int categoryId, bool notInUse)
            {
                Product[] products = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { categoryId } } }).ToArray();
                if (notInUse)
                    foreach (Product p in products)
                        productsToSiteMap.Add((p.Id, false, null));
                else
                    foreach (Product p in products)
                    {
                        if (!p.NotInUse)
                            productsToSiteMap.Add((p.Id, true, null));
                    }

            }
            if (category.NotInUse)
                await AllNotInUse(_service.GetModels().Where(c => c.ParentId == category.Id).ToList());
            else
            {
                if (category.ParentId != 0)
                {
                    Category parent = await _service.GetModelAsync(category.ParentId);
                    if (parent.NotInUse)
                        category.NotInUse = parent.NotInUse;
                }
            }
            await _service.EditModelAsync(category, images, WIDTH, HEIGHT);
            await _service.AddCategoryAdLinksAsync(category.Id, adLinks);
            await EditActAsync(category.Id, category.NameRu);
            if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { category.Id } } }).Any())
                CorrectProductsSiteMap(category.Id, category.NotInUse);
            categoriesToSiteMap.Add((category.Id, !category.NotInUse, null));
            _entry.CacheForSiteMap(ConstantsService.CATEGORY, categoriesToSiteMap);
            if (productsToSiteMap.Any())
                _entry.CacheForSiteMap(ConstantsService.PRODUCT, productsToSiteMap);
            _cacheClean.CleanCategory(category.Id);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Category category = await _service.GetModelAsync(id);
            if (category == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(category.Id, category);
            if (result == Result.Success)
                await DeleteActAsync(id, category.NameRu);
            _cacheClean.CleanCategory(category.Id);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return result;
        }
    }
}
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
        private readonly ICategory _category;
        private readonly IProduct _product;
        private readonly ICacheClean _cacheClean;
        public CategoryController(IEntry entry, ICategory category, IProduct product, ICacheClean cacheClean) : base(entry, Entity.Category, category)
        {
            _category = category;
            _product = product;
            _cacheClean = cacheClean;
        }

        [HttpGet("{id}")]
        public async Task<Category> Get(int id)
        {
            Category category = await _category.GetModelAsync(id);
            return category;
        }
        [HttpGet("hasproduct/{id}")]
        public async Task<bool> HasProduct(int id)
        {
            return await _category.HasProduct(id);
        }
        [HttpGet("tree")]
        public async Task<IEnumerable<AdminCategoryTree>> GetTree()
        {
            IEnumerable<AdminCategoryTree> categories = await _category.GetAdminCategoryTree();
            return categories;
        }
        [HttpGet("adlinks/{id}")]
        public async Task<string[]> GetAdLinks(int id)
        {
            return await _category.GetAdLinksAsync(id);
        }
        [HttpGet("modeladlinks/{id}")]
        public async Task<string[]> GetModelAdLinks(int id)
        {
            return await _category.GetModelAdLinksAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Category category, [FromForm] int[] adLinks, [FromForm] IFormFile[] images)
        {
            bool isExist = _category.IsExist(category, _category.GetCategoriesByParentId(category.ParentId));
            if (isExist)
                return Result.Already;
            if (category.IsForHome)
            {
                bool isImaged = _category.IsCategoryImaged(images);
                if (!isImaged)
                    return Result.NoImage;
            }
            Category parent = await _category.GetModelAsync(category.ParentId);
            if (parent != null)
            {
                if (parent.NotInUse)
                {
                    category.NotInUse = true;
                }
            }
            await _category.AddModelAsync(category, images, WIDTH, HEIGHT);
            await _category.AddCategoryAdLinksAsync(category.Id, adLinks);
            await AddActAsync(category.Id, category.NameRu);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Category category, [FromForm] int[] adLinks, [FromForm] IFormFile[] images)
        {
            bool isExist = _category.IsExist(category, _category.GetModels().Where(x => x.Id != category.Id));
            if (isExist)
                return Result.Already;
            if (category.IsForHome)
            {
                bool isImaged = _category.IsCategoryImaged(images, category.Id);
                if (!isImaged)
                    return Result.NoImage;
            }
            IList<(int, bool)> categoriesToSiteMap = new List<(int, bool)>();
            IList<(int, bool)> productsToSiteMap = new List<(int, bool)>();
            async Task AllNotInUse(IEnumerable<Category> categories)
            {
                if (categories.Any())
                    foreach (Category category in categories)
                    {
                        category.NotInUse = true;
                        categoriesToSiteMap.Add((category.Id, !category.NotInUse));
                        if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { category.Id } } }).Any())
                            CorrectProductsSiteMap(category.Id, category.NotInUse);
                        await AllNotInUse(_category.GetModels().Where(c => c.ParentId == category.Id));
                    }
            }
            void CorrectProductsSiteMap(int categoryId, bool notInUse)
            {
                Product[] products = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { categoryId } } }).ToArray();
                if (notInUse)
                    foreach (Product p in products)
                        productsToSiteMap.Add((p.Id, false));
                else
                    foreach (Product p in products)
                    {
                        if (!p.NotInUse)
                            productsToSiteMap.Add((p.Id, true));
                    }

            }
            if (category.NotInUse)
                await AllNotInUse(_category.GetModels().Where(c => c.ParentId == category.Id).ToList());
            else
            {
                if (category.ParentId != 0)
                {
                    Category parent = await _category.GetModelAsync(category.ParentId);
                    if (parent.NotInUse)
                        category.NotInUse = parent.NotInUse;
                }
            }
            await _category.EditModelAsync(category, images, WIDTH, HEIGHT);
            await _category.AddCategoryAdLinksAsync(category.Id, adLinks);
            await EditActAsync(category.Id, category.NameRu);
            if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { category.Id } } }).Any())
                CorrectProductsSiteMap(category.Id, category.NotInUse);
            categoriesToSiteMap.Add((category.Id, !category.NotInUse));
            await _entry.CorrectSiteMap(ConstantsService.CATEGORY, categoriesToSiteMap);
            if (productsToSiteMap.Any())
                await _entry.CorrectSiteMap(ConstantsService.PRODUCT, productsToSiteMap);
            _cacheClean.CleanCategory(category.Id);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Category category = await _category.GetModelAsync(id);
            if (category == null)
                return Result.Fail;
            Result result = await _category.DeleteModelAsync(category.Id, category);
            if (result == Result.Success)
                await DeleteActAsync(id, category.NameRu);
            _cacheClean.CleanCategory(category.Id);
            _cacheClean.CleanCategories(category.ParentId);
            _cacheClean.CleanIndexCategoriesPromotions();
            return result;
        }
    }
}
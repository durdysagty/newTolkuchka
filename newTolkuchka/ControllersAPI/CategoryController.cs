using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
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
        public CategoryController(IEntry entry, ICategory category) : base(entry, Entity.Category, category)
        {
            _category = category;
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
            void AllNotInUse(IEnumerable<Category> categories)
            {
                if (categories.Any())
                    foreach (Category category in categories)
                    {
                        category.NotInUse = true;
                        AllNotInUse(_category.GetModels().Where(c => c.ParentId == category.Id));
                    }
            }
            if (category.NotInUse)
                AllNotInUse(_category.GetModels().Where(c => c.ParentId == category.Id));
            await _category.EditModelAsync(category, images, WIDTH, HEIGHT);
            await _category.AddCategoryAdLinksAsync(category.Id, adLinks);
            await EditActAsync(category.Id, category.NameRu);
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
            return result;
        }
    }
}
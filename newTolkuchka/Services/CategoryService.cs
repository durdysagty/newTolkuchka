using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class CategoryService : ServiceFormFile<Category, AdminCategory>, ICategory
    {
        // private const int PADDING = 2;
        private readonly IProduct _product;
        private readonly IMemoryCache _memoryCache;
        public CategoryService(AppDbContext con, IProduct product, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, IPath path, ICacheClean cacheClean, IImage image) : base(con, localizer, path, cacheClean, image, ConstantsService.UMAXIMAGE)
        {
            _product = product;
            _memoryCache = memoryCache;
        }

        public async Task<bool> HasProduct(int id)
        {
            return await GetProducts(id).AnyAsync();
        }

        public async Task<IEnumerable<AdminCategoryTree>> GetAdminCategoryTree(int startId)
        {
            IEnumerable<AdminCategoryTree> CreateTree(IEnumerable<Category> categories, int parentId, int level)
            {
                IEnumerable<AdminCategoryTree> tree = categories.Where(x => x.ParentId == parentId).OrderBy(x => x.Order).Select(x => new AdminCategoryTree
                {
                    Id = x.Id,
                    Level = level,
                    Name = x.NameRu,
                    HasProduct = x.Models.Any(),
                    List = CreateTree(categories, x.Id, level + 1)
                });
                return tree;
            }
            IEnumerable<AdminCategoryTree> categoryTrees = CreateTree(await GetFullModels().ToArrayAsync(), startId, 0);
            return categoryTrees;
        }

        public async Task<IEnumerable<CategoryTree>> GetCategoryTree(int depth)
        {
            IEnumerable<CategoryTree> CreateTree(IEnumerable<Category> categories, int parentId, int level)
            {
                IEnumerable<CategoryTree> tree = categories.Where(x => (x.ParentId == parentId || x.CategoryAdLinks.Any(a => a.StepParentId == parentId)) && !x.NotInUse).OrderBy(x => x.Order).Select(x => new CategoryTree
                {
                    Id = x.Id,
                    Name = CultureProvider.GetLocalName(x.NameRu, x.NameEn, x.NameTm),
                    List = level < depth ? CreateTree(categories, x.Id, level + 1) : null
                });
                return tree;
            }
            IEnumerable<CategoryTree> categoryTrees = CreateTree(await GetModels().Include(x => x.CategoryAdLinks).ToArrayAsync(), 0, 0);
            return categoryTrees;
        }

        public IQueryable<Category> GetCategoriesByParentId(int parentId)
        {
            IQueryable<Category> categories = GetModels().Where(c => (c.ParentId == parentId || c.CategoryAdLinks.Where(y => y.StepParentId == parentId).Any())).OrderBy(x => x.Order);
            return categories;
        }

        public IQueryable<Category> GetActiveCategoriesByParentId(int parentId)
        {
            // no need to cache a query
            IQueryable<Category> categories = GetCategoriesByParentId(parentId).Where(c => !c.NotInUse);
            return categories;
        }

        public async Task<IList<int>> GetAllCategoryIdsHaveProductsByParentIdCachedAsync(int parentId)
        {
            IList<int> ids = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORYIDSGOTPRODUCTSBYPARENTID}{parentId}", async ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                IList<int> ids = new List<int>();
                async Task GetAll(int catId)
                {
                    IList<Category> categories = await GetActiveCategoriesByParentId(catId).Include(c => c.Models).ThenInclude(m => m.Products).ToListAsync();
                    if (categories.Any())
                        foreach (Category c in categories)
                        {
                            if (c.Models.Select(m => m.Products).Any())
                                ids.Add(c.Id);
                            else
                                await GetAll(c.Id);
                        }
                }
                IQueryable<Product> products = GetProducts(parentId);
                if (products.Any())
                    ids.Add(parentId);
                else
                    await GetAll(parentId);
                return ids;
            });
            return ids;
        }

        public IOrderedQueryable<Category> GetIndexCategories()
        {
            IOrderedQueryable<Category> categories = GetModels().Where(c => c.IsForHome && !c.NotInUse).OrderBy(c => c.ParentId).ThenBy(c => c.Order);
            return categories;
        }
        public async Task<string[]> GetAdLinksAsync(int id)
        {
            return await GetCategoryAdLinks(id).Select(x => x.StepParentId.ToString()).ToArrayAsync();
        }

        public async Task AddCategoryAdLinksAsync(int id, IList<int> adLinks)
        {
            IList<CategoryAdLink> categoryAdLinks = await GetCategoryAdLinks(id).ToListAsync();
            IList<CategoryAdLink> toRemove = categoryAdLinks.Where(x => !adLinks.Contains(x.StepParentId)).ToList();
            foreach (var adLink in toRemove)
            {
                _con.CategoryAdLinks.Remove(adLink);
            }
            IList<int> toAdds = adLinks.Where(x => !categoryAdLinks.Select(y => y.StepParentId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                CategoryAdLink categoryAdLink = new()
                {
                    CategoryId = id,
                    StepParentId = toAdd
                };
                await _con.CategoryAdLinks.AddAsync(categoryAdLink);
            }
        }

        public async Task<string[]> GetModelAdLinksAsync(int id)
        {
            return await GetCategoryModelAdLinks(id).Select(x => x.CategoryId.ToString()).ToArrayAsync();
        }

        public async Task AddCategoryModelAdLinksAsync(int id, IList<int> adLinks)
        {
            IList<CategoryModelAdLink> categoryModelAdLinks = await GetCategoryModelAdLinks(id).ToListAsync();
            IList<CategoryModelAdLink> toRemove = categoryModelAdLinks.Where(x => !adLinks.Contains(x.CategoryId)).ToList();
            foreach (var adLink in toRemove)
            {
                _con.CategoryModelAdLinks.Remove(adLink);
            }
            IList<int> toAdds = adLinks.Where(x => !categoryModelAdLinks.Select(y => y.CategoryId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                CategoryModelAdLink categoryModelAdLink = new()
                {
                    ModelId = id,
                    CategoryId = toAdd
                };
                await _con.CategoryModelAdLinks.AddAsync(categoryModelAdLink);
            }
        }

        public bool IsCategoryImaged(IFormFile[] images, int id)
        {
            if (images.Any(i => i.Length > 0))
                return true;
            if (id > 0)
            {
                string[] files = Directory.GetFiles($"{_path.GetImagesFolder()}/{ConstantsService.CATEGORY}", $"{id}-0.jpg", SearchOption.TopDirectoryOnly);
                if (files.Any())
                    return true;
            }
            return false;
        }

        private IQueryable<CategoryAdLink> GetCategoryAdLinks(int id)
        {
            return _con.CategoryAdLinks.Where(x => x.CategoryId == id);
        }
        private IQueryable<CategoryModelAdLink> GetCategoryModelAdLinks(int id)
        {
            return _con.CategoryModelAdLinks.Where(x => x.ModelId == id);

        }

        private IQueryable<Product> GetProducts(int id)
        {
            return _product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new[] { id } } });
        }
    }
}

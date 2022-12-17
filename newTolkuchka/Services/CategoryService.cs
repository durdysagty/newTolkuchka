using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class CategoryService : ServiceNoFile<Category>, ICategory
    {
        private const int PADDING = 2;
        private readonly IProduct _product;
        public CategoryService(AppDbContext con, IStringLocalizer<Shared> localizer, IProduct product) : base(con, localizer)
        {
            _product = product;
        }

        public async Task<IEnumerable<AdminCategory>> GetAdminCategories()
        {
            IEnumerable<Category> list = await GetModels().ToArrayAsync();
            List<AdminCategory> categories = new();
            void GetCategoriesByOrder(IEnumerable<Category> parentList, int level)
            {

                foreach (var c in parentList)
                {
                    categories.Add(new AdminCategory
                    {
                        Padding = level * PADDING,
                        Id = c.Id,
                        Name = c.Order + " " + c.NameRu,
                        Products = GetProducts(c.Id).Count()
                    });
                    GetCategoriesByOrder(list.Where(x => x.ParentId == c.Id).OrderBy(x => x.Order), level + 1);
                }
            }
            GetCategoriesByOrder(list.Where(x => x.ParentId == 0).OrderBy(x => x.Order), 0);
            return categories;
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
                    HasProduct = GetProducts(x.Id).Any(),
                    List = CreateTree(categories, x.Id, level + 1)
                });
                return tree;
            }
            IEnumerable<AdminCategoryTree> categoryTrees = CreateTree(await GetModels().ToArrayAsync(), startId, 0);
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
            IQueryable<Category> categories = GetModels().Where(c => (c.ParentId == parentId || c.CategoryAdLinks.Where(y => y.StepParentId == parentId).Any()) && !c.NotInUse).OrderBy(x => x.Order);
            return categories;
        }

        public IList<int> GetAllCategoryIdsHaveProductsByParentId(int parentId)
        {
            IList<int> ids = new List<int>();
            void GetAll(int catId)
            {
                IQueryable<Category> categories = GetCategoriesByParentId(catId).Include(c => c.Products);
                if (categories.Any())
                    foreach (Category c in categories)
                    {
                        if (c.Products.Any())
                            ids.Add(c.Id);
                        else
                            GetAll(c.Id);
                    }
            }
            IQueryable<Product> products = GetProducts(parentId);
            if (products.Any())
                ids.Add(parentId);
            else
                GetAll(parentId);
            return ids;
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

        public async Task<string[]> GetProductAdLinksAsync(int id) //toremove
        {
            return await GetCategoryProductAdLinks(id).Select(x => x.CategoryId.ToString()).ToArrayAsync();
        }

        public async Task AddCategoryProductAdLinksAsync(int id, IList<int> adLinks) //to remove
        {
            IList<CategoryProductAdLink> categoryProductAdLinks = await GetCategoryProductAdLinks(id).ToListAsync();
            IList<CategoryProductAdLink> toRemove = categoryProductAdLinks.Where(x => !adLinks.Contains(x.CategoryId)).ToList();
            foreach (var adLink in toRemove)
            {
                _con.CategoryProductAdLinks.Remove(adLink);
            }
            IList<int> toAdds = adLinks.Where(x => !categoryProductAdLinks.Select(y => y.CategoryId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                CategoryProductAdLink categoryProductAdLink = new()
                {
                    ProductId = id,
                    CategoryId = toAdd
                };
                await _con.CategoryProductAdLinks.AddAsync(categoryProductAdLink);
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

        private IQueryable<CategoryAdLink> GetCategoryAdLinks(int id)
        {
            return _con.CategoryAdLinks.Where(x => x.CategoryId == id);
        }

        private IQueryable<CategoryProductAdLink> GetCategoryProductAdLinks(int id) //to remove
        {
            return _con.CategoryProductAdLinks.Where(x => x.ProductId == id);

        }
        private IQueryable<CategoryModelAdLink> GetCategoryModelAdLinks(int id)
        {
            return _con.CategoryModelAdLinks.Where(x => x.ModelId == id);

        }

        private IQueryable<Product> GetProducts(int id)
        {
            return _product.GetProducts(new[] { id });
        }
    }
}

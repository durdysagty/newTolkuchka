using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ICategory : IActionFormFile<Category, AdminCategory>
    {
        Task<bool> HasProduct(int id);
        Task<IEnumerable<AdminCategoryTree>> GetAdminCategoryTree(int startId = 0);
        Task<IEnumerable<CategoryTree>> GetCategoryTree(int depth = 3);
        IQueryable<Category> GetCategoriesByParentId(int parentId);
        IQueryable<Category> GetActiveCategoriesByParentId(int parentId);
        Task<IList<int>> GetAllCategoryIdsHaveProductsByParentIdCachedAsync(int parentId);
        Task<IList<Category>> GetIndexCategories();
        Task<string[]> GetAdLinksAsync(int id);
        Task AddCategoryAdLinksAsync(int id, IList<int> adLinks);
        Task<string[]> GetModelAdLinksAsync(int id);
        Task AddCategoryModelAdLinksAsync(int id, IList<int> adLinks);
        bool IsCategoryImaged(IFormFile[] images, int id = 0);
    }
}

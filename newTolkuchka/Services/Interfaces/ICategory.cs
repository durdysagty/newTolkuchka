using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ICategory : IActionNoFile<Category>
    {
        Task<IEnumerable<AdminCategory>> GetAdminCategories();
        Task<bool> HasProduct(int id);
        Task<IEnumerable<AdminCategoryTree>> GetAdminCategoryTree(int startId = 0);
        Task<IEnumerable<CategoryTree>> GetCategoryTree(int depth = 3);
        IQueryable<Category> GetCategoriesByParentId(int parentId);
        IList<int> GetAllCategoryIdsHaveProductsByParentId(int parentId);
        Task<string[]> GetAdLinksAsync(int id);
        Task AddCategoryAdLinksAsync(int id, IList<int> adLinks);
        Task<string[]> GetModelAdLinksAsync(int id);
        Task AddCategoryModelAdLinksAsync(int id, IList<int> adLinks);
    }
}

using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IBreadcrumbs
    {
        Task<IList<Breadcrumb>> GetCategoryBreadcrumbsAsync(int parentId);
        IList<Breadcrumb> GetModelBreadcrumbs(string model);
        // parentId is category or brand id to create parent link
        // name is brand name to create link from brand
        // isBrand is used for to determine is product page came from category or brand
        Task<IList<Breadcrumb>> GetProductBreadcrumbs(int parentId, string name = null, bool isBrand = false);
        IList<Breadcrumb> GetBreadcrumbs();
    }
}
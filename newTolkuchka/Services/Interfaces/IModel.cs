using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IModel : IActionNoFile<Model, AdminModel>
    {
        Task<IList<int[]>> GetModelSpecsForAdminAsync(int id);
        Task AddModelSpecsAsync(int id, IList<int[]> specs);
        IQueryable<ModelSpec> GetModelSpecs(int id, bool isNameUse = false);
    }
}

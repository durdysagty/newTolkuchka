using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IModel : IActionNoFile<Model, AdminModel>
    {
        Task<IList<int[]>> GetModelSpecsForAdminAsync(int id);
        IQueryable<ModelSpec> GetModelSpecs(int id, bool isNameUse = false);
        Task<object[]> GetSpecValuesAsync(int id);
        Task<object[]> GetSpecValueModsAsync(int id);
        Task AddModelSpecsAsync(int id, IList<int[]> specs);
    }
}

using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IModel : IActionNoFile<Model>
    {
        IEnumerable<AdminModel> GetAdminModels(int[] brandId, int?[] lineId, int page, int pp, out int lastPage, out string pagination);
        Task<IList<int[]>> GetModelSpecsForAdminAsync(int id);
        Task AddModelSpecsAsync(int id, IList<int[]> specs);
        IQueryable<ModelSpec> GetModelSpecs(int id, bool isNameUse = false);
    }
}

using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IModel : IActionNoFile<Model>
    {
        IEnumerable<AdminModel> GetAdminModels([FromQuery] int[] brandId, [FromQuery] int?[] lineId);
        Task<IList<int[]>> GetModelSpecsForAdminAsync(int id);
        Task AddModelSpecsAsync(int id, IList<int[]> specs);
        IQueryable<ModelSpec> GetModelSpecs(int id, bool isNameUse = false);
        //IQueryable<int> GetModelSpecsSpecIds(int id, bool isNameUse = false);
    }
}

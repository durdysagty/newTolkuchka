using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISpec : IActionNoFile<Spec, AdminSpec>
    {
        IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecWithValues(int modelId = 0);
        Task<bool> IsSpecImaged(int id);
    }
}

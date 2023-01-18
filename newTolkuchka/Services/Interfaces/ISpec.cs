using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISpec : IActionNoFile<Spec, AdminSpec>
    {
        IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecWithValues(Dictionary<string, object> paramsList = null);
        Task<bool> IsSpecImaged(int id);
    }
}

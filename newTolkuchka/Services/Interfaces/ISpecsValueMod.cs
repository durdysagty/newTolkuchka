using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISpecsValueMod : IActionNoFile<SpecsValueMod>
    {
        IEnumerable<AdminSpecsValueMod> GetAdminSpecsValueMods(int specId);
    }
}

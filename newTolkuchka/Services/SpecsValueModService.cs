using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecsValueModService : ServiceNoFile<SpecsValueMod>, ISpecsValueMod
    {
        public SpecsValueModService(AppDbContext con) : base(con)
        {
        }

        public IEnumerable<AdminSpecsValueMod> GetAdminSpecsValueMods(int specValueId)
        {
            IEnumerable<AdminSpecsValueMod> specsValueMods = GetModels().Where(x => x.SpecsValueId == specValueId).Select(x => new AdminSpecsValueMod
            {
                Id = x.Id,
                Name = x.NameRu,
                Products = x.ProductSpecsValueMods.Count
            }).OrderBy(x => x.Name);
            return specsValueMods;
        }
    }
}

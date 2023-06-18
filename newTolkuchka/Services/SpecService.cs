using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecService : ServiceNoFile<Spec, AdminSpec>, ISpec
    {
        public SpecService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, memoryCache, localizer, cacheClean)
        {
        }
        public IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecWithValues(Dictionary<string, object> paramsList = null)
        {
            // may be GetFullModels in include all stuff & Dictionary instead of modelId
            IQueryable<Spec> specs = GetModels().Include(s => s.SpecsValues).ThenInclude(sv => sv.SpecsValueMods).Include(s=> s.ModelSpecs);

            if (paramsList.TryGetValue(ConstantsService.MODEL, out object value))
            {
                int modelId = int.Parse(value.ToString());
                specs = specs.Where(s => s.ModelSpecs.Where(x => x.ModelId == modelId).Any());
            }
            if (paramsList.TryGetValue(ConstantsService.SPEC, out value))
            {
                IList<int> specIds = (IList<int>)value;
                if (specIds != null)
                    specs = specs.Where(s => specIds.Any(x => x == s.Id));
            }
            IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> specWithValues = specs.OrderBy(x => x.Order).Select(x => new ModelWithList<ModelWithList<AdminSpecsValueMod>>
            {
                Id = x.Id,
                Name = x.NameRu,
                List = x.SpecsValues.Select(x => new ModelWithList<AdminSpecsValueMod>
                {
                    Id = x.Id,
                    Name = x.NameRu,
                    List = x.SpecsValueMods.Select(x => new AdminSpecsValueMod
                    {
                        Id = x.Id,
                        Name = x.NameRu
                    }).ToList()
                }).ToList()
            });
            return specWithValues;
        }

        public async Task<bool> IsSpecImaged(int id)
        {
            return await Task.FromResult(GetModelAsync(id).Result.IsImaged);
        }
    }
}
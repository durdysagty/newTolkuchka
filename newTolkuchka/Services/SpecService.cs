using Microsoft.EntityFrameworkCore;
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
        public SpecService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }
        public IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecWithValues(int modelId = 0)
        {
            // may be GetFullModels in include all stuff & Dictionary instead of modelId
            IQueryable<Spec> specs = GetModels().Include(s => s.SpecsValues).ThenInclude(sv => sv.SpecsValueMods).Include(s=> s.ModelSpecs);
            if (modelId != 0)
                specs = specs.Where(s => s.ModelSpecs.Where(x => x.ModelId == modelId).Any());
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
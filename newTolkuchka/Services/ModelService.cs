using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ModelService : ServiceNoFile<Model, AdminModel>, IModel
    {
        private readonly IProduct _product;
        public ModelService(AppDbContext con, IProduct product, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
            _product = product;
        }

        public async Task<IList<int[]>> GetModelSpecsForAdminAsync(int id)
        {
            return await GetModelSpecs(id).Select(x => new int[] { x.SpecId, x.IsNameUse ? 1 : 0 }).ToListAsync();
        }

        public async Task AddModelSpecsAsync(int id, IList<int[]> specs)
        {
            IList<ModelSpec> modelSpecs = await GetModelSpecs(id).ToListAsync();
            IList<ModelSpec> toRemove = modelSpecs.Where(x => !specs.Select(x => x[0]).Contains(x.SpecId)).ToList();
            foreach (ModelSpec r in toRemove)
            {
                _product.RemoveProductSpecValuesModelSpecRemovedAsync(id, r.SpecId);
                _con.ModelSpecs.Remove(r);
            }
            IEnumerable<ModelSpec> toEdits = modelSpecs.Except(toRemove);
            foreach (ModelSpec toEdit in toEdits)
                toEdit.IsNameUse = specs.FirstOrDefault(x => x[0] == toEdit.SpecId)[1] > 0;
            IList<int[]> toAdds = specs.Where(x => !modelSpecs.Select(y => y.SpecId).Contains(x[0])).ToList();
            foreach (int[] toAdd in toAdds)
            {
                ModelSpec ms = new()
                {
                    ModelId = id,
                    SpecId = toAdd[0],
                    IsNameUse = toAdd[1] > 0,
                };
                await _con.ModelSpecs.AddAsync(ms);
            }
        }
        public IQueryable<ModelSpec> GetModelSpecs(int id, bool isNameUse = false)
        {
            var modelSpecs = _con.ModelSpecs.Where(x => x.ModelId == id);
            if (isNameUse)
                modelSpecs = modelSpecs.Where(x => x.IsNameUse);
            return modelSpecs;
        }

        //public IQueryable<int> GetModelSpecsSpecIds(int id, bool isNameUse = false)
        //{
        //    return GetModelSpecs(id, isNameUse).Select(x => x.SpecId);
        //}
    }
}

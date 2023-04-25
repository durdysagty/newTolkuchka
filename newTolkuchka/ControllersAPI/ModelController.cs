using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class ModelController : AbstractController<Model, AdminModel, IModel>
    {
        private readonly ICategory _category;
        private readonly IProduct _product;
        public ModelController(IEntry entry, IModel model, ICacheClean cacheClean, ICategory category, IProduct product) : base(entry, Entity.Model, model, cacheClean)
        {
            _category = category;
            _product = product;
        }

        [HttpGet("{id}")]
        public async Task<Model> Get(int id)
        {
            Model model = await _service.GetModelAsync(id);
            return model;
        }
        [HttpGet("specs/{id}")]
        public async Task<IList<int[]>> GetModelSpecs(int id)
        {
            return await _service.GetModelSpecsForAdminAsync(id);
        }
        [HttpGet("specvalues/{id}")]
        public async Task<object[]> GetSpecValues(int id)
        {
            return await _service.GetSpecValuesAsync(id);
        }
        [HttpGet("specvaluemods/{id}")]
        public async Task<object[]> GetSpecValueMods(int id)
        {
            return await _service.GetSpecValueModsAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Model model, [FromForm] IList<int[]> specs, [FromForm] int[] adLinks)
        {
            bool isExist = _service.IsExist(model, _service.GetModels().Where(x => x.LineId == model.LineId && x.BrandId == model.BrandId && x.TypeId == model.TypeId));
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(model);
            if (specs.Any())
                await _service.AddModelSpecsAsync(model.Id, specs);
            if (adLinks.Any())
                await _category.AddCategoryModelAdLinksAsync(model.Id, adLinks);
            await AddActAsync(model.Id, model.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Model model, [FromForm] IList<int[]> specs, [FromForm] int[] adLinks, [FromForm] IList<int[]> productSpecsValues, [FromForm] IList<int[]> productSpecsValueMods)
        {
            bool isExist = _service.IsExist(model, _service.GetModels().Where(x => x.LineId == model.LineId && x.BrandId == model.BrandId && x.TypeId == model.TypeId && x.Id != model.Id));
            if (isExist)
                return Result.Already;
            int[]productIds = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.MODEL, model.Id } }).Select(p => p.Id).ToArray();
            if (productIds.Any())
            {
                // get product ids from all productSpecsValues
                // create productId with specsvalues
                IList<(int, int[])> productSpecsValuesCheck = new List<(int, int[])>();
                foreach (int id in productIds)
                {
                    int[] specValueIds = productSpecsValues.Where(psv => psv[0] == id).Select(psv => psv[1]).ToArray();
                    productSpecsValuesCheck.Add((id, specValueIds));
                    // clean cached pages
                    _cacheClean.CleanProductPage(id);
                }
                // test if there are equal sequances of specsvalues
                if (productIds.Length > 1)
                    for (int i = 0; i < productSpecsValuesCheck.Count; i++)
                    {
                        IList<(int, int[])> test = new List<(int, int[])>(productSpecsValuesCheck);
                        test.RemoveAt(i);
                        bool result = _product.IsSequencesEqual(productSpecsValuesCheck[i].Item2, test.Select(t => t.Item2));
                        if (result)
                            return Result.Already;
                    }
                foreach ((int, int[]) psv in productSpecsValuesCheck)
                {
                    await _product.AddProductSpecValuesAsync(psv.Item1, psv.Item2);
                }

                // same process but productSpecsValueMods & no need to check
                IList<(int, int[])> productSpecsValueModsCheck = new List<(int, int[])>();
                foreach (int id in productIds)
                {
                    int[] specValueIds = productSpecsValueMods.Where(psvm => psvm[0] == id).Select(psvm => psvm[1]).ToArray();
                    productSpecsValueModsCheck.Add((id, specValueIds));
                }
                foreach ((int, int[]) psvm in productSpecsValueModsCheck)
                {
                    await _product.AddProductSpecValueModsAsync(psvm.Item1, psvm.Item2);
                }
            }
            _service.EditModel(model);
            await _service.AddModelSpecsAsync(model.Id, specs);
            await _category.AddCategoryModelAdLinksAsync(model.Id, adLinks);
            await EditActAsync(model.Id, model.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            // do not need clean cache of products b.o. before delete model you have to delete all products of given model
            Model model = await _service.GetModelAsync(id);
            if (model == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(model.Id, model);
            if (result == Result.Success)
                await DeleteActAsync(id, model.Name);
            return result;
        }
    }
}
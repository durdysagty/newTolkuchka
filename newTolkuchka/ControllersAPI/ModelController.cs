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
        private readonly IModel _model;
        private readonly ICategory _category;
        private readonly IProduct _product;
        public ModelController(IEntry entry, IModel model, ICategory category, IProduct product) : base(entry, Entity.Model, model)
        {
            _model = model;
            _category = category;
            _product = product;
        }

        [HttpGet("{id}")]
        public async Task<Model> Get(int id)
        {
            Model model = await _model.GetModelAsync(id);
            return model;
        }
        //[HttpGet]
        //public ModelsFilters<AdminModel> Get(/*[FromQuery] int[] brand, [FromQuery] int?[] line, */[FromQuery] int page = 0, [FromQuery] int pp = 50)
        //{
        //    IEnumerable<AdminModel> models = _model.GetAdminModels(/*brand, line, */page, pp, out int lastPage, out string pagination);
        //    return new ModelsFilters<AdminModel>
        //    {
        //        //Filters = new string[2] { nameof(brand), $"{nameof(brand)}Id {nameof(line)}" }.OrderBy(c => c),
        //        Models = models,
        //        LastPage = lastPage,
        //        Pagination = pagination
        //    };
        //}
        [HttpGet("specs/{id}")]
        public async Task<IList<int[]>> GetModelSpecs(int id)
        {
            return await _model.GetModelSpecsForAdminAsync(id);
        }
        [HttpGet("specvalues/{id}")]
        public async Task<object[]> GetSpecValues(int id)
        {
            return await _model.GetSpecValuesAsync(id);
        }
        [HttpGet("specvaluemods/{id}")]
        public async Task<object[]> GetSpecValueMods(int id)
        {
            return await _model.GetSpecValueModsAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Model model, [FromForm] IList<int[]> specs, [FromForm] int[] adLinks)
        {
            bool isExist = _model.IsExist(model, _model.GetModels().Where(x => x.LineId == model.LineId));
            if (isExist)
                return Result.Already;
            await _model.AddModelAsync(model);
            if (specs.Any())
                await _model.AddModelSpecsAsync(model.Id, specs);
            if (adLinks.Any())
                await _category.AddCategoryModelAdLinksAsync(model.Id, adLinks);
            await AddActAsync(model.Id, model.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Model model, [FromForm] IList<int[]> specs, [FromForm] int[] adLinks, [FromForm] IList<int[]> productSpecsValues, [FromForm] IList<int[]> productSpecsValueMods)
        {
            bool isExist = _model.IsExist(model, _model.GetModels().Where(x => x.LineId == model.LineId && x.Id != model.Id));
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
            _model.EditModel(model);
            await _model.AddModelSpecsAsync(model.Id, specs);
            await _category.AddCategoryModelAdLinksAsync(model.Id, adLinks);
            await EditActAsync(model.Id, model.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Model model = await _model.GetModelAsync(id);
            if (model == null)
                return Result.Fail;
            Result result = await _model.DeleteModelAsync(model.Id, model);
            if (result == Result.Success)
                await DeleteActAsync(id, model.Name);
            return result;
        }
    }
}
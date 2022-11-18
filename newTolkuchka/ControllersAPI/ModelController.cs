using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class ModelController : AbstractController
    {
        private readonly IModel _model;
        public ModelController(IEntry entry, IModel model) : base(entry, Entity.Model)
        {
            _model = model;
        }

        [HttpGet("{id}")]
        public async Task<Model> Get(int id)
        {
            Model model = await _model.GetModelAsync(id);
            return model;
        }
        [HttpGet]
        public ModelsFilters<AdminModel> Get([FromQuery] int[] brand, [FromQuery] int?[] line, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            IEnumerable<AdminModel> models = _model.GetAdminModels(brand, line, page, pp, out int lastPage, out string pagination);
            return new ModelsFilters<AdminModel>
            {
                Filters = new string[2] { nameof(brand), $"{nameof(brand)}Id {nameof(line)}" }.OrderBy(c => c),
                Models = models,
                LastPage = lastPage,
                Pagination = pagination
            };
        }
        [HttpGet("specs/{id}")]
        public async Task<IList<int[]>> GetModelSpecs(int id)
        {
            return await _model.GetModelSpecsForAdminAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Model model, [FromForm] IList<int[]> specs)
        {
            bool isExist = _model.IsExist(model, _model.GetModels().Where(x => x.LineId == model.LineId));
            if (isExist)
                return Result.Already;
            await _model.AddModelAsync(model);
            if (specs.Any())
                await _model.AddModelSpecsAsync(model.Id, specs);
            await AddActAsync(model.Id, model.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Model model, [FromForm] IList<int[]> specs)
        {
            bool isExist = _model.IsExist(model, _model.GetModels().Where(x => x.LineId == model.LineId && x.Id != model.Id));
            if (isExist)
                return Result.Already;
            _model.EditModel(model);
            await _model.AddModelSpecsAsync(model.Id, specs);
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
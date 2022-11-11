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
        public IEnumerable<AdminModel> Get(int? brandId, int? lineId)
        {
            IEnumerable<AdminModel> models = _model.GetAdminModels(brandId, lineId);
            return models;
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
            Result result = await _model.DeleteModel(model.Id, model);
            if (result == Result.Success)
                await DeleteActAsync(id, model.Name);
            return result;
        }
    }
}
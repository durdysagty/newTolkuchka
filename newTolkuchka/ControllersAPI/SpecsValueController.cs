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
    public class SpecsValueController : AbstractController<SpecsValue, AdminSpecsValue, ISpecsValue>
    {
        private const int WIDTH = 25;
        private const int HEIGHT = 25;
        private readonly ISpecsValue _specsValue;
        public SpecsValueController(IEntry entry, ISpecsValue specsValue) : base(entry, Entity.SpecsValue, specsValue)
        {
            _specsValue = specsValue;
        }

        [HttpGet("{id}")]
        public async Task<SpecsValue> Get(int id)
        {
            SpecsValue specsValue = await _specsValue.GetModelAsync(id);
            return specsValue;
        }
        [HttpGet($"{ConstantsService.SPEC}/{{specId}}")]
        public ModelsFilters<AdminSpecsValue> GetBySpec(int specId, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            IEnumerable<AdminSpecsValue> specsValues = _specsValue.GetAdminModels(page, pp, out int lastPage, out string pagination, new Dictionary<string, object> { { ConstantsService.SPEC, specId } });
            return new ModelsFilters<AdminSpecsValue>
            {
                Models = specsValues,
                LastPage = lastPage,
                Pagination = pagination
            };
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] SpecsValue specsValue, [FromForm] IFormFile[] images)
        {
            bool isExist = _specsValue.IsExist(specsValue, _specsValue.GetModels().Where(x => x.SpecId == specsValue.SpecId));
            if (isExist)
                return Result.Already;
            await _specsValue.AddModelAsync(specsValue, images, WIDTH, HEIGHT);
            await AddActAsync(specsValue.Id, specsValue.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] SpecsValue specsValue, [FromForm] IFormFile[] images)
        {
            bool isExist = _specsValue.IsExist(specsValue, _specsValue.GetModels().Where(x => x.SpecId == specsValue.SpecId && x.Id != specsValue.Id));
            if (isExist)
                return Result.Already;
            await _specsValue.EditModelAsync(specsValue, images, WIDTH, HEIGHT);
            await EditActAsync(specsValue.Id, specsValue.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            SpecsValue SpecsValue = await _specsValue.GetModelAsync(id);
            if (SpecsValue == null)
                return Result.Fail;
            Result result = await _specsValue.DeleteModelAsync(SpecsValue.Id, SpecsValue);
            if (result == Result.Success)
                await DeleteActAsync(id, SpecsValue.NameRu);
            return result;
        }
    }
}
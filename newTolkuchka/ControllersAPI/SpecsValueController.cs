using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SpecsValueController : AbstractController
    {
        private const int WIDTH = 25;
        private const int HEIGHT = 25;
        private readonly ISpecsValue _specsValue;
        public SpecsValueController(IEntry entry, ISpecsValue SpecsValue) : base(entry, Entity.SpecsValue)
        {
            _specsValue = SpecsValue;
        }

        [HttpGet("{id}")]
        public async Task<SpecsValue> Get(int id)
        {
            SpecsValue specsValue = await _specsValue.GetModelAsync(id);
            return specsValue;
        }
        [HttpGet("spec/{specId}")]
        public IEnumerable<AdminSpecsValue> GetBySpec(int specId)
        {
            IEnumerable<AdminSpecsValue> specsValues = _specsValue.GetAdminSpecsValues(specId);
            return specsValues;
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
            Result result = await _specsValue.DeleteModel(SpecsValue.Id, SpecsValue);
            if (result == Result.Success)
                await DeleteActAsync(id, SpecsValue.NameRu);
            return result;
        }
    }
}
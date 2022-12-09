using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SpecsValueModController : AbstractController<SpecsValueMod, ISpecsValueMod>
    {
        private readonly ISpecsValueMod _specsValueMod;
        public SpecsValueModController(IEntry entry, ISpecsValueMod specsValueMod) : base(entry, Entity.SpecsValueMod, specsValueMod)
        {
            _specsValueMod = specsValueMod;
        }

        [HttpGet("{id}")]
        public async Task<SpecsValueMod> Get(int id)
        {
            SpecsValueMod specsValueMod = await _specsValueMod.GetModelAsync(id);
            return specsValueMod;
        }
        [HttpGet("specsvalue/{specValueId}")]
        public IEnumerable<AdminSpecsValueMod> GetBySpecsValue(int specValueId)
        {
            IEnumerable<AdminSpecsValueMod> specsValueMods = _specsValueMod.GetAdminSpecsValueMods(specValueId);
            return specsValueMods;
        }
        [HttpPost]
        public async Task<Result> Post(SpecsValueMod specsValueMod)
        {
            bool isExist = _specsValueMod.IsExist(specsValueMod, _specsValueMod.GetModels());
            if (isExist)
                return Result.Already;
            await _specsValueMod.AddModelAsync(specsValueMod);
            await AddActAsync(specsValueMod.Id, specsValueMod.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(SpecsValueMod specsValueMod)
        {
            bool isExist = _specsValueMod.IsExist(specsValueMod, _specsValueMod.GetModels().Where(x => x.Id != specsValueMod.Id));
            if (isExist)
                return Result.Already;
            _specsValueMod.EditModel(specsValueMod);
            await EditActAsync(specsValueMod.Id, specsValueMod.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            SpecsValueMod SpecsValueMod = await _specsValueMod.GetModelAsync(id);
            if (SpecsValueMod == null)
                return Result.Fail;
            Result result = await _specsValueMod.DeleteModelAsync(SpecsValueMod.Id, SpecsValueMod);
            if (result == Result.Success)
                await DeleteActAsync(id, SpecsValueMod.NameRu);
            return result;
        }
    }
}
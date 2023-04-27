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
    public class SpecsValueModController : AbstractController<SpecsValueMod, AdminSpecsValueMod, ISpecsValueMod>
    {
        public SpecsValueModController(IEntry entry, ISpecsValueMod specsValueMod, ICacheClean cacheClean) : base(entry, Entity.SpecsValueMod, specsValueMod, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<SpecsValueMod> Get(int id)
        {
            SpecsValueMod specsValueMod = await _service.GetModelAsync(id);
            return specsValueMod;
        }
        //[HttpGet("specsvalue/{specValueId}")]
        //public IEnumerable<AdminSpecsValueMod> GetBySpecsValue(int specValueId)
        //{
        //    IEnumerable<AdminSpecsValueMod> specsValueMods = _service.GetAdminSpecsValueMods(specValueId);
        //    return specsValueMods;
        //}
        //[HttpGet($"{ConstantsService.SPECSVALUE}/{{specValueId}}")]
        //public ModelsFilters<AdminSpecsValueMod> GetBySpec(int specValueId, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        //{
        //    IEnumerable<AdminSpecsValueMod> specsValueMods = _service.GetAdminModels(page, pp, out int lastPage, out string pagination, new Dictionary<string, object> { { ConstantsService.SPECSVALUE, specValueId } });
        //    return new ModelsFilters<AdminSpecsValueMod>
        //    {
        //        Models = specsValueMods,
        //        LastPage = lastPage,
        //        Pagination = pagination
        //    };
        //}
        [HttpPost]
        public async Task<Result> Post(SpecsValueMod specsValueMod)
        {
            bool isExist = _service.IsExist(specsValueMod, _service.GetModels().Where(x => x.SpecsValueId == specsValueMod.SpecsValueId));
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(specsValueMod);
            await AddActAsync(specsValueMod.Id, specsValueMod.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(SpecsValueMod specsValueMod)
        {
            bool isExist = _service.IsExist(specsValueMod, _service.GetModels().Where(x => x.Id != specsValueMod.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(specsValueMod);
            await EditActAsync(specsValueMod.Id, specsValueMod.NameRu);
            _cacheClean.CleanProductPage();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            SpecsValueMod SpecsValueMod = await _service.GetModelAsync(id);
            if (SpecsValueMod == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(SpecsValueMod.Id, SpecsValueMod);
            if (result == Result.Success)
                await DeleteActAsync(id, SpecsValueMod.NameRu);
            return result;
        }
    }
}
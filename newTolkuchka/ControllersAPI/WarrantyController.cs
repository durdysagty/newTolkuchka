using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class WarrantyController : AbstractController<Warranty, AdminWarranty, IWarranty>
    {
        public WarrantyController(IEntry entry, IWarranty warranty, ICacheClean cacheClean) : base(entry, Entity.Warranty, warranty, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Warranty> Get(int id)
        {
            Warranty warranty = await _service.GetModelAsync(id);
            return warranty;
        }
        [HttpPost]
        public async Task<Result> Post(Warranty warranty)
        {
            bool isExist = _service.IsExist(warranty, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(warranty);
            await AddActAsync(warranty.Id, warranty.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Warranty warranty)
        {
            bool isExist = _service.IsExist(warranty, _service.GetModels().Where(x => x.Id != warranty.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(warranty);
            await EditActAsync(warranty.Id, warranty.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Warranty warranty = await _service.GetModelAsync(id);
            if (warranty == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(warranty.Id, warranty);
            if (result == Result.Success)
                await DeleteActAsync(id, warranty.NameRu);
            return result;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class WarrantyController : AbstractController
    {
        private readonly IWarranty _warranty;
        public WarrantyController(IEntry entry, IWarranty warranty) : base(entry, Entity.Warranty)
        {
            _warranty = warranty;
        }

        [HttpGet("{id}")]
        public async Task<Warranty> Get(int id)
        {
            Warranty warranty = await _warranty.GetModelAsync(id);
            return warranty;
        }
        [HttpGet]
        public IEnumerable<AdminWarranty> Get()
        {
            IEnumerable<AdminWarranty> warranties = _warranty.GetAdminWarranties();
            return warranties;
        }
        [HttpPost]
        public async Task<Result> Post(Warranty warranty)
        {
            bool isExist = _warranty.IsExist(warranty, _warranty.GetModels());
            if (isExist)
                return Result.Already;
            await _warranty.AddModelAsync(warranty);
            await AddActAsync(warranty.Id, warranty.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Warranty warranty)
        {
            bool isExist = _warranty.IsExist(warranty, _warranty.GetModels().Where(x => x.Id != warranty.Id));
            if (isExist)
                return Result.Already;
            _warranty.EditModel(warranty);
            await EditActAsync(warranty.Id, warranty.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Warranty warranty = await _warranty.GetModelAsync(id);
            if (warranty == null)
                return Result.Fail;
            Result result = await _warranty.DeleteModel(warranty.Id, warranty);
            if (result == Result.Success)
                await DeleteActAsync(id, warranty.NameRu);
            return result;
        }
    }
}
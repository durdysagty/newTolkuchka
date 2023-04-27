using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class SupplierController : AbstractController<Supplier, AdminSupplier, ISupplier>
    {
        public SupplierController(IEntry entry, ISupplier supplier, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Supplier, supplier, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Supplier> Get(int id)
        {
            Supplier supplier = await _service.GetModelAsync(id);
            return supplier;
        }
        //[HttpGet]
        //public IEnumerable<AdminSupplier> Get()
        //{
        //    IEnumerable<AdminSupplier> suppliers = _service.GetAdminSuppliers();
        //    return suppliers;
        //}
        [HttpPost]
        public async Task<Result> Post(Supplier supplier)
        {
            bool isExist = _service.IsExist(supplier, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(supplier);
            await AddActAsync(supplier.Id, supplier.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Supplier supplier)
        {
            bool isExist = _service.IsExist(supplier, _service.GetModels().Where(x => x.Id != supplier.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(supplier);
            await EditActAsync(supplier.Id, supplier.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Supplier supplier = await _service.GetModelAsync(id);
            if (supplier == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(supplier.Id, supplier);
            if (result == Result.Success)
                await DeleteActAsync(id, supplier.Name);
            return result;
        }
    }
}
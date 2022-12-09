using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class SupplierController : AbstractController<Supplier, ISupplier>
    {
        private readonly ISupplier _supplier;
        public SupplierController(IEntry entry, ISupplier supplier) : base(entry, Entity.Supplier, supplier)
        {
            _supplier = supplier;
        }

        [HttpGet("{id}")]
        public async Task<Supplier> Get(int id)
        {
            Supplier supplier = await _supplier.GetModelAsync(id);
            return supplier;
        }
        [HttpGet]
        public IEnumerable<AdminSupplier> Get()
        {
            IEnumerable<AdminSupplier> suppliers = _supplier.GetAdminSuppliers();
            return suppliers;
        }
        [HttpPost]
        public async Task<Result> Post(Supplier supplier)
        {
            bool isExist = _supplier.IsExist(supplier, _supplier.GetModels());
            if (isExist)
                return Result.Already;
            await _supplier.AddModelAsync(supplier);
            await AddActAsync(supplier.Id, supplier.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Supplier supplier)
        {
            bool isExist = _supplier.IsExist(supplier, _supplier.GetModels().Where(x => x.Id != supplier.Id));
            if (isExist)
                return Result.Already;
            _supplier.EditModel(supplier);
            await EditActAsync(supplier.Id, supplier.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Supplier supplier = await _supplier.GetModelAsync(id);
            if (supplier == null)
                return Result.Fail;
            Result result = await _supplier.DeleteModelAsync(supplier.Id, supplier);
            if (result == Result.Success)
                await DeleteActAsync(id, supplier.Name);
            return result;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Type = newTolkuchka.Models.Type;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class TypeController : AbstractController<Type, AdminType, IType>
    {
        private readonly IProduct _product;
        public TypeController(IEntry entry, IType type, IMemoryCache memoryCache, ICacheClean cacheClean, IProduct product) : base(entry, Entity.Type, type, memoryCache, cacheClean)
        {
            _product = product;
        }

        [HttpGet("{id}")]
        public async Task<Type> Get(int id)
        {
            Type Type = await _service.GetModelAsync(id);
            return Type;
        }
        //[HttpGet]
        //public IEnumerable<AdminType> Get()
        //{
        //    IEnumerable<AdminType> types = _service.GetAdminTypes();
        //    return types;
        //}
        [HttpPost]
        public async Task<Result> Post(Type type)
        {
            bool isExist = _service.IsExist(type, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(type);
            await AddActAsync(type.Id, type.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Type type)
        {
            bool isExist = _service.IsExist(type, _service.GetModels().Where(x => x.Id != type.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(type);
            await EditActAsync(type.Id, type.NameRu);
            // clean cached Products Page
            _cacheClean.CleanAllModeledProducts();
            // clean cached Products Page
            int[] productIds = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.TYPE, type.Id } }).Select(p => p.Id).ToArray();
            foreach (int id in productIds)
                _cacheClean.CleanProductPage(id);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Type Type = await _service.GetModelAsync(id);
            if (Type == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(Type.Id, Type);
            if (result == Result.Success)
                await DeleteActAsync(id, Type.NameRu);
            return result;
        }
    }
}
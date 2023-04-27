using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Type = newTolkuchka.Models.Type;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class TypeController : AbstractController<Type, AdminType, IType>
    {
        public TypeController(IEntry entry, IType type, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Type, type, memoryCache, cacheClean)
        {
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
            _cacheClean.CleanProductPage();
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
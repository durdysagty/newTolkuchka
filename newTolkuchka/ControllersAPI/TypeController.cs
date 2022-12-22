using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IType _type;
        public TypeController(IEntry entry, IType type) : base(entry, Entity.Type, type)
        {
            _type = type;
        }

        [HttpGet("{id}")]
        public async Task<Type> Get(int id)
        {
            Type Type = await _type.GetModelAsync(id);
            return Type;
        }
        //[HttpGet]
        //public IEnumerable<AdminType> Get()
        //{
        //    IEnumerable<AdminType> types = _type.GetAdminTypes();
        //    return types;
        //}
        [HttpPost]
        public async Task<Result> Post(Type type)
        {
            bool isExist = _type.IsExist(type, _type.GetModels());
            if (isExist)
                return Result.Already;
            await _type.AddModelAsync(type);
            await AddActAsync(type.Id, type.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Type type)
        {
            bool isExist = _type.IsExist(type, _type.GetModels().Where(x => x.Id != type.Id));
            if (isExist)
                return Result.Already;
            _type.EditModel(type);
            await EditActAsync(type.Id, type.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Type Type = await _type.GetModelAsync(id);
            if (Type == null)
                return Result.Fail;
            Result result = await _type.DeleteModelAsync(Type.Id, Type);
            if (result == Result.Success)
                await DeleteActAsync(id, Type.NameRu);
            return result;
        }
    }
}
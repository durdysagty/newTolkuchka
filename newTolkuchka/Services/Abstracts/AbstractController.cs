using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    [ApiController, Route("api/[controller]")]
    public abstract class AbstractController<TModel, TService> : ControllerBase
    {
        private protected readonly IEntry _entry;
        private readonly Entity _entity;
        private readonly TService _service;
        public AbstractController(IEntry entry, Entity entity, TService service)
        {
            _entry = entry;
            _entity = entity;
            _service = service;
        }

        private protected async Task AddActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Add, _entity, entityId, entityName);
        }
        private protected async Task EditActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Edit, _entity, entityId, entityName);
        }
        private protected async Task DeleteActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Delete, _entity, entityId, entityName);
        }

        [HttpGet("{id}/{key}")]
        public async Task<Result> ChangeNotInUse(int id, string key)
        {
            Type modelType = typeof(TModel);
            Type serviceType = typeof(TService);
            MethodInfo method = modelType.Name == "Product" ? serviceType.GetMethod("GetFullProductAsync", new Type[1] { typeof(int) }) :  serviceType.GetInterface("IAction`1").GetMethod("GetModelAsync", new Type[1] {typeof(int)});
            object result = method.Invoke(_service, new object[] {id});
            TModel model = await (Task<TModel>)result;
            PropertyInfo property = modelType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(model, !(bool)property.GetValue(model));
            await EditActAsync(id, modelType.Name == "Product" ? IProduct.GetProductName(model as Product): modelType.GetProperty("Name").GetValue(model).ToString());
            return Result.Success;
        }
    }
}

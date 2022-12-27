using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    [ApiController, Route("api/[controller]")]
    public abstract class AbstractController<TModel, TAdminModel, TService> : ControllerBase
    {
        private protected readonly IEntry _entry;
        //private protected readonly IAction<TModel, TAdminModel> _action;
        private protected readonly Entity _entity;
        private protected readonly TService _service;
        public AbstractController(IEntry entry, Entity entity, TService service)
        {
            _entry = entry;
            _entity = entity;
            _service = service;
        }

        [HttpGet]
        public ModelsFilters<TAdminModel> Get([FromQuery] string search, [FromQuery] string[] keys, [FromQuery] string[] values, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            Dictionary<string, object> paramsList = null;
            if (keys.Any())
            {
                paramsList = new();
                for (int i = 0; i < keys.Length; i++)
                    paramsList.Add(keys[i], values[i]);
            }
            if (search != null)
            {
                paramsList ??= new();
                paramsList.Add(nameof(search), search);
            }
            Type serviceType = typeof(TService);
            MethodInfo method = serviceType.GetInterface("IAction`2").GetMethod("GetAdminModels", new Type[5] { typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(string).MakeByRefType(), typeof(Dictionary<string, object>) });
            object[] args = new object[5] { page, pp, null, null, paramsList };
            object result = method.Invoke(_service, args);
            IEnumerable<TAdminModel> models = (IEnumerable<TAdminModel>)result;
            return new ModelsFilters<TAdminModel>
            {
                Models = models,
                LastPage = (int)args[2],
                Pagination = (string)args[3]
            };
        }

        [HttpGet("{id}/{key}")]
        public async Task<Result> ChangeNotInUse(int id, string key)
        {
            Type modelType = typeof(TModel);
            Type serviceType = typeof(TService);
            MethodInfo method = modelType.Name == "Product" ? serviceType.GetMethod("GetFullProductAsync", new Type[1] { typeof(int) }) : serviceType.GetInterface("IAction`1").GetMethod("GetModelAsync", new Type[1] { typeof(int) });
            object result = method.Invoke(_service, new object[] { id });
            TModel model = await (Task<TModel>)result;
            PropertyInfo property = modelType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(model, !(bool)property.GetValue(model));
            await EditActAsync(id, modelType.Name == "Product" ? IProduct.GetProductName(model as Product) : modelType.GetProperty("Name").GetValue(model).ToString());
            return Result.Success;
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
    }
}

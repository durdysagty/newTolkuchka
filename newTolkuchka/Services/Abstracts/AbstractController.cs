using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
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
        private protected readonly ICacheClean _cacheClean;
        public AbstractController(IEntry entry, Entity entity, TService service, ICacheClean cacheClean)
        {
            _entry = entry;
            _entity = entity;
            _service = service;
            _cacheClean = cacheClean;
        }

        [HttpGet]
        public ModelsFilters<TAdminModel> Get([FromQuery] string search, [FromQuery] string[] keys, [FromQuery] string[] values, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            if (typeof(TAdminModel).Name == ConstantsService.ADMINREPORTORDER)
            {
                IMemoryCache _memoryCache = HttpContext.RequestServices.GetService<IMemoryCache>();
                string cacheKey = string.Empty;
                cacheKey = $"{ConstantsService.ADMINREPORTORDER}-{values[0]}-{values[1]}";
                return _memoryCache.GetOrCreate(cacheKey, ce =>
                {
                    ce.SlidingExpiration = TimeSpan.FromDays(3);
                    ModelsFilters<TAdminModel> response = CreateResponse(search, keys, values, page, pp);
                    if (!_memoryCache.TryGetValue(ConstantsService.ADMINREPORTSHASHKEYS, out HashSet<string> reportKeys))
                        reportKeys = new HashSet<string>();
                    reportKeys.Add(cacheKey);
                    _memoryCache.Set(ConstantsService.ADMINREPORTSHASHKEYS, reportKeys, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromDays(10)
                    });
                    return response;
                });
            }
            ModelsFilters<TAdminModel> response = CreateResponse(search, keys, values, page, pp);
            return response;
        }

        [HttpGet("{id}/{key}")]
        public async Task<Result> ChangeBoolenProperty(int id, string key)
        {
            Type modelType = typeof(TModel);
            if (modelType.Name == "Invoice")
                return Result.Fail;
            Type serviceType = typeof(TService);
            MethodInfo method = modelType.Name == "Product" ? serviceType.GetMethod("GetFullProductAsync", new Type[1] { typeof(int) }) : serviceType.GetInterfaces().FirstOrDefault(i => i.Name.Contains("IAction`")).GetMethod("GetModelAsync", new Type[1] { typeof(int) });
            object result = method.Invoke(_service, new object[] { id });
            TModel model = await (Task<TModel>)result;
            PropertyInfo property = modelType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            bool value = (bool)property.GetValue(model);
            property.SetValue(model, !value);
            if (modelType.Name == "Product")
            {
                _cacheClean.CleanProductPage(id);
                await EditActAsync(id, IProduct.GetProductNameCounted(model as Product));
                if (property.Name == "NotInUse")
                    await _entry.CorrectSiteMap(ConstantsService.PRODUCT, new (int, bool)[] { (id, value) });
            }
            else
            {
                PropertyInfo nameProperty = modelType.GetProperty("Name");
                if (nameProperty == null)
                    nameProperty = modelType.GetProperty("NameRu");
                await EditActAsync(id, nameProperty.GetValue(model).ToString());
            }
            return Result.Success;
        }
        #region actions
        private protected async Task AddActAsync(int entityId, string entityName, CultureProvider.Culture? culture = null)
        {
            await _entry.AddEntryAsync(Act.Add, _entity, entityId, entityName, null, culture);
        }
        private protected async Task EditActAsync(int entityId, string entityName, bool? siteMapToAdd = null, CultureProvider.Culture? culture = null)
        {
            await _entry.AddEntryAsync(Act.Edit, _entity, entityId, entityName, siteMapToAdd, culture);
        }
        private protected async Task DeleteActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Delete, _entity, entityId, entityName);
        }
        #endregion
        private ModelsFilters<TAdminModel> CreateResponse(string search, string[] keys, string[] values, int page, int pp)
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
    }
}

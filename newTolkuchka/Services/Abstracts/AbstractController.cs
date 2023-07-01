using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using System.Text;
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
        private protected readonly IMemoryCache _memoryCache;
        private protected readonly ICacheClean _cacheClean;
        public AbstractController(IEntry entry, Entity entity, TService service, IMemoryCache memoryCache, ICacheClean cacheClean)
        {
            _entry = entry;
            _entity = entity;
            _service = service;
            _memoryCache = memoryCache;
            _cacheClean = cacheClean;
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public ModelsFilters<TAdminModel> Get([FromQuery] string search, [FromQuery] string[] keys, [FromQuery] string[] values, [FromQuery] int page = 0, [FromQuery] int pp = 50)
        {
            bool isKeysExist;
            string cacheKey;
            if (typeof(TAdminModel).Name == ConstantsService.ADMINREPORTORDER)
            {
                // create a key that will be used to get a report
                StringBuilder reportKeyBuilder = new($"{ConstantsService.ADMINREPORTORDER}-{values[0]}-{values[1]}");
                cacheKey = reportKeyBuilder.ToString();
                // get all keys in memory that is used for get adminreport models, if not then create
                isKeysExist = _memoryCache.TryGetValue(ConstantsService.ADMINREPORTSHASHKEYS, out HashSet<string> reportKeys);
                if (!isKeysExist)
                    reportKeys = new HashSet<string>();
                // check is the key are included to reportKeys and try get the models from cache
                if (!(reportKeys.Contains(cacheKey) && _memoryCache.TryGetValue(cacheKey, out ModelsFilters<TAdminModel> report)))
                {
                    // if not in modelKeys or not in cache memory
                    report = CreateResponse(search, keys, values, page, pp);
                    _memoryCache.Set(cacheKey, report, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromDays(3)
                    });
                    // also have to add the key to modelKeys & set keys to memory again
                    reportKeys.Add(cacheKey);
                    _memoryCache.Set(ConstantsService.ADMINREPORTSHASHKEYS, reportKeys, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove
                    });
                }
                return report;
            }
            // create a key that will be used to get models
            StringBuilder stringBuilder = new($"{typeof(TModel).Name}-{search}");
            if (values.Any())
                foreach (var value in values)
                    stringBuilder.Append($"-{value}");
            stringBuilder.Append($"-{page}-{pp}");
            cacheKey = stringBuilder.ToString();
            // get all keys in memory that is used for get models, if not then create
            isKeysExist = _memoryCache.TryGetValue(ConstantsService.ADMINMODELSHASHKEYS, out HashSet<string> modelKeys);
            if (!isKeysExist)
                modelKeys = new HashSet<string>();
            // check is the key are included to modelKeys and try get the models from cache
            if (!(modelKeys.Contains(cacheKey) && _memoryCache.TryGetValue(cacheKey, out ModelsFilters<TAdminModel> response)))
            {
                // if not in modelKeys or not in cache memory
                response = CreateResponse(search, keys, values, page, pp);
                MemoryCacheEntryOptions cacheEntryOptions = new();
                if (typeof(TModel).Name == ConstantsService.USER)
                {
                    cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(20);
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                }
                else
                {
                    cacheEntryOptions.SlidingExpiration = TimeSpan.FromHours(2);
                }
                _memoryCache.Set(cacheKey, response, cacheEntryOptions);
                // also have to add the key to modelKeys & set keys to memory again
                modelKeys.Add(cacheKey);
                _memoryCache.Set(ConstantsService.ADMINMODELSHASHKEYS, modelKeys, new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove
                });
            }
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
                _cacheClean.CleanAdminModels(modelType.Name);
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

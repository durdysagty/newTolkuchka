using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using static newTolkuchka.Services.CultureProvider;

namespace newTolkuchka.Services
{
    public class EntryService : ServiceNoFile<Entry, AdminEntry>, IEntry
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPath _path;
        private readonly IMemoryCache _memoryCache;
        //private readonly IEmployee _employee;
        public EntryService(IHttpContextAccessor contextAccessor, IPath path, AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
            _contextAccessor = contextAccessor;
            _path = path;
            _memoryCache = memoryCache;
        }

        public async Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName, bool? siteMapToAdd = null, Culture? culture = null)
        {
            Entry entry = new()
            {
                Employee = IContext.GetAthorizedUserId(_contextAccessor.HttpContext),
                Act = act,
                Entity = entity,
                EntityId = entityId,
                DateTime = DateTimeOffset.UtcNow.ToUniversalTime(),
                EntityName = entityName
            };
            await AddModelAsync(entry, true);
            // adding new urls to sitemap
            if (entity is Entity.Product or Entity.Promotion or Entity.Category or Entity.Brand or Entity.Article)
            {
                if (act == Act.Add)
                    CacheForSiteMap(entity.ToString(), new (int, bool, Culture?)[] { (entityId, true, culture) });
                if (act == Act.Delete)
                    CacheForSiteMap(entity.ToString(), new (int, bool, Culture?)[] { (entityId, false, culture) });
                if (act == Act.Edit && siteMapToAdd != null)
                {
                    if ((bool)siteMapToAdd)
                        CacheForSiteMap(entity.ToString(), new (int, bool, Culture?)[] { (entityId, true, culture) , });
                    else
                        CacheForSiteMap(entity.ToString(), new (int, bool, Culture?)[] { (entityId, false, culture) });
                }
            }
        }

        public void CacheForSiteMap(string entity, IList<(int, bool, Culture?)> entities)
        {
            string model = entity.ToLower();
            string path = _path.GetSiteMap(model);
            bool isCached = _memoryCache.TryGetValue(path, out IList<(int, bool, Culture?)> memoryEntities);
            if (!isCached)
            {
                _memoryCache.Set(path, entities, new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove
                });
            }
            else
            {
                memoryEntities = entities.Concat(memoryEntities).DistinctBy(e => e.Item1).ToList();
                _memoryCache.Set(path, memoryEntities, new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove
                });
            }
            //List<string> strings = File.ReadAllLines(path).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            //foreach ((int, bool) e in entities)
            //{
            //    string modelUrl = $"/{PathService.GetModelUrl(model, e.Item1)}";
            //    List<string> urls = new();
            //    if (culture == null)
            //    {
            //        urls = new List<string> { $"{CultureProvider.SiteUrlRu}{modelUrl}", $"{CultureProvider.SiteUrlEn}{modelUrl}", $"{CultureProvider.SiteUrlTm}{modelUrl}" };
            //    }
            //    else
            //    {
            //        // used for articles, i.e. they are only for current culture
            //        if (culture == CultureProvider.Culture.Ru)
            //            urls = new List<string> { $"{CultureProvider.SiteUrlRu}{modelUrl}" };
            //        if (culture == CultureProvider.Culture.En)
            //            urls = new List<string> { $"{CultureProvider.SiteUrlEn}{modelUrl}" };
            //        if (culture == CultureProvider.Culture.Tm)
            //            urls = new List<string> { $"{CultureProvider.SiteUrlTm}{modelUrl}" };
            //    }
            //    if (e.Item2)
            //    {
            //        if (!strings.Any(s => s == urls[0]))
            //            strings = strings.Concat(urls).ToList();
            //    }
            //    else
            //        foreach (string u in urls)
            //            strings.Remove(strings.FirstOrDefault(s => s == u));
            //}
            //// replace to remove
            //await File.WriteAllLinesAsync(path, strings.OrderByDescending(s => s, new CompareForSiteMapService()));
        }
    }
}

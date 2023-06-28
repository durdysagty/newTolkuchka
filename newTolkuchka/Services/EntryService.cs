using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EntryService : ServiceNoFile<Entry, AdminEntry>, IEntry
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPath _path;
        //private readonly IEmployee _employee;
        public EntryService(IHttpContextAccessor contextAccessor, IPath path, AppDbContext con,/* IEmployee employee,*/  IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
            _contextAccessor = contextAccessor;
            _path = path;
        }

        public async Task AddEntryAsync(Act act, Entity entity, int entityId, string entityName, bool? siteMapToAdd = null, CultureProvider.Culture? culture = null)
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
                    await CorrectSiteMap(entity.ToString(), new (int, bool)[] { (entityId, true) }, culture);
                if (act == Act.Delete)
                    await CorrectSiteMap(entity.ToString(), new (int, bool)[] { (entityId, false) }, culture);
                if (act == Act.Edit && siteMapToAdd != null)
                {
                    if ((bool)siteMapToAdd)
                        await CorrectSiteMap(entity.ToString(), new (int, bool)[] { (entityId, true) });
                    else
                        await CorrectSiteMap(entity.ToString(), new (int, bool)[] { (entityId, false) });
                }
            }
        }

        // externally we use this method only in Category controller, b.o. category's NotInUse state act on Product
        // if not in use we have to remove or add if in use state
        // sitemap not nessaserily to be ordered, b.o. edit correction
        //public void CorrectSiteMap(string entity, int entityId, bool add)
        //{
        //    string model = entity.ToLower();
        //    string modelUrl = $"/{PathService.GetModelUrl(model, entityId)}";
        //    string[] urls = { $"{CultureProvider.SiteUrlRu}{modelUrl}", $"{CultureProvider.SiteUrlEn}{modelUrl}", $"{CultureProvider.SiteUrlTm}{modelUrl}" };
        //    string path = _path.GetSiteMap(model);
        //    List<string> strings = File.ReadAllLines(path).ToList();
        //    if (add)
        //    {
        //        if (!strings.Any(s => s == urls[0]))
        //        {
        //            strings = strings.Concat(urls).ToList();
        //            File.WriteAllLines(path, strings.OrderBy(s => s.Reverse().ToString()));
        //        }
        //    }
        //    else
        //    {
        //        foreach (string u in urls)
        //        {
        //            strings.Remove(strings.FirstOrDefault(s => s == u));
        //        }
        //        File.WriteAllLines(path, strings.OrderBy(s => s.Reverse().ToString()));
        //    }
        //}

        public async Task CorrectSiteMap(string entity, IList<(int, bool)> entities, CultureProvider.Culture? culture = null)
        {
            string model = entity.ToLower();
            string path = _path.GetSiteMap(model);
            List<string> strings = File.ReadAllLines(path).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            foreach ((int, bool) e in entities)
            {
                string modelUrl = $"/{PathService.GetModelUrl(model, e.Item1)}";
                List<string> urls = new();
                if (culture == null)
                {
                    urls = new List<string> { $"{CultureProvider.SiteUrlRu}{modelUrl}", $"{CultureProvider.SiteUrlEn}{modelUrl}", $"{CultureProvider.SiteUrlTm}{modelUrl}" };
                }
                else
                {
                    if (culture == CultureProvider.Culture.Ru)
                        urls = new List<string> { $"{CultureProvider.SiteUrlRu}{modelUrl}" };
                    if (culture == CultureProvider.Culture.En)
                        urls = new List<string> { $"{CultureProvider.SiteUrlEn}{modelUrl}" };
                    if (culture == CultureProvider.Culture.Tm)
                        urls = new List<string> { $"{CultureProvider.SiteUrlTm}{modelUrl}" };
                }
                if (e.Item2)
                {
                    if (!strings.Any(s => s == urls[0]))
                        strings = strings.Concat(urls).ToList();
                }
                else
                    foreach (string u in urls)
                        strings.Remove(strings.FirstOrDefault(s => s == u));
            }
            // replace to remove
            await File.WriteAllLinesAsync(path, strings.OrderByDescending(s => s, new CompareForSiteMapService()));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using static newTolkuchka.Services.CultureProvider;
using Type = System.Type;

namespace newTolkuchka.Services
{
    public class SiteMapService : ISiteMap
    {
        private readonly IPath _path;
        private readonly AppDbContext _con;

        public SiteMapService(IPath path, AppDbContext con)
        {
            _path = path;
            _con = con;
        }

        public async Task RenewSiteMap()
        {
            string[] modelsToSiteMap = { Entity.Product.ToString().ToLower(), Entity.Promotion.ToString().ToLower(), Entity.Category.ToString().ToLower(), Entity.Brand.ToString().ToLower(), Entity.Article.ToString().ToLower() };
            foreach (var m in modelsToSiteMap)
            {
                IList<string> urls = new List<string>();
                string path = _path.GetSiteMap(m);
                if (m != ConstantsService.ARTICLE)
                {
                    int[] ids = m switch
                    {
                        ConstantsService.PRODUCT => _con.Products.Where(p => !p.NotInUse && !p.Model.Category.NotInUse).Select(p => p.Id).ToArray(),
                        ConstantsService.PROMOTION => _con.Promotions.Where(p => !p.NotInUse).Select(p => p.Id).ToArray(),
                        ConstantsService.CATEGORY => _con.Categories.Where(c => !c.NotInUse).Select(p => p.Id).ToArray(),
                        _ => _con.Brands.Select(p => p.Id).ToArray(),
                    };
                    foreach (int i in ids)
                    {
                        string modelUrl = $"/{PathService.GetModelUrl(m, i)}";
                        urls = urls.Concat(new List<string> { $"{SiteUrlRu}{modelUrl}", $"{SiteUrlEn}{modelUrl}", $"{SiteUrlTm}{modelUrl}" }).ToList();
                    }
                }
                else
                {
                    Article[] articles = _con.Articles.Include(a => a.HeadingArticles).ThenInclude(ha => ha.Heading).ToArray();
                    foreach (var article in articles)
                    {
                        string modelUrl = $"/{PathService.GetModelUrl(m, article.Id)}";
                        string url = article.HeadingArticles.FirstOrDefault().Heading.Language switch
                        {
                            Culture.En => $"{SiteUrlEn}{modelUrl}",
                            Culture.Tm => $"{SiteUrlTm}{modelUrl}",
                            _ => $"{SiteUrlRu}{modelUrl}"
                        };
                        urls.Add(url);
                    }
                }
                await File.WriteAllLinesAsync(path, urls.OrderByDescending(s => s, new CompareForSiteMapService()));
            }
        }
    }
}

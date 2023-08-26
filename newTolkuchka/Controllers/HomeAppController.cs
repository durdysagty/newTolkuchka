using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeAppController : AbstractHomeController
    {
        public HomeAppController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IPromotion promotion, IModel model, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, ICustomerGuid customerGuid, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache, ICrypto crypto, ILogger<HomeController> logger) : base(localizer, breadcrumbs, path, category, brand, promotion, model, product, slide, heading, article, user, customerGuid, invoice, order, login, currency, memoryCache, crypto, logger)
        {
        }

#if !DEBUG
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
#else
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
        public async Task<IList<Category>> Main()
        {
            //HttpContext.Request.Cookies.TryGetValue("w", out string width);
            IList<Category> categories = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORIESGROUPBYPARENTID}{0}", async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromDays(10);
                return await _category.GetActiveCategoriesByParentId(0).ToListAsync();
            });
            return categories;
        }

#if !DEBUG
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
#else
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
#endif
        public async Task<JsonResult> Items()
        {
            // cleaned at CleanIndexCategoriesPromotions()
            IList<Category> categories = await _memoryCache.GetOrCreateAsync(ConstantsService.INDEXAPPCATS, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromDays(10);
                return await _category.GetIndexCategories();
            });
            IList<AdminBrand> brands = await _memoryCache.GetOrCreate(ConstantsService.HOMEAPPBRANDS, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IList<AdminBrand> brands = await _brand.GetModels().Where(b => b.IsForHome).Select(b => new AdminBrand
                {
                    Id = b.Id,
                    Name = b.Name,
                    Version = b.Version
                }).ToListAsync();
                return brands;
            });
            IList<Slide> slides = await _memoryCache.GetOrCreate(ConstantsService.HOMEAPPSLIDES, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IList<Slide> slides = await _slide.GetSlidesByLayoutAsync(Layout.Main).OrderByDescending(s => s.Id).Take(2).ToListAsync();
                return slides;
            });
            int count = 7;
            IList<AppIndexItem> appIndexItems = new List<AppIndexItem>();
            List<IEnumerable<UIProduct>> mobileUIProducts = await GetIndexSelectedCatProducts(count);
            Category selectedCategory = await _category.GetModelAsync(selectedCategoryId);
            AppIndexItem mobileProducts = new()
            {
                ProductsModel = new AppProductsModel
                {
                    Model = ConstantsService.CATEGORY,
                    Id = selectedCategoryId,
                    ModelName = CultureProvider.GetLocalName(selectedCategory.NameRu, selectedCategory.NameEn, selectedCategory.NameTm)
                },
                Products = mobileUIProducts
            };
            appIndexItems.Add(mobileProducts);
            List<IEnumerable<UIProduct>> newUIProducts = await GetIndexNewProducts(count);
            AppIndexItem newProducts = new()
            {
                ProductsModel = new AppProductsModel
                {
                    Model = ConstantsService.NOVELTIES,
                    ModelName = _localizer[ConstantsService.NOVELTIES].Value
                },
                Products = newUIProducts
            };
            appIndexItems.Add(newProducts);
            List<IEnumerable<UIProduct>> recUIProducts = await GetIndexRecProducts(count);
            AppIndexItem recProducts = new()
            {
                ProductsModel = new AppProductsModel
                {
                    Model = ConstantsService.RECOMMENDED,
                    ModelName = _localizer[ConstantsService.RECOMMENDED].Value
                },
                Products = recUIProducts
            };
            appIndexItems.Add(recProducts);
            return new JsonResult(new
            {
                categories,
                brands,
                slides,
                appIndexItems
            });
        }

#if !DEBUG
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
#endif
        public async Task<IEnumerable<CategoryTree>> Categories()
        {
            IEnumerable<CategoryTree> categories = await _memoryCache.GetOrCreateAsync(ConstantsService.CATEGORIES, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(3);
                return await _category.GetCategoryTree();
            });
            return categories;
        }

#if !DEBUG
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
#endif
        public async Task<IList<Brand>> Brands()
        {
            IList<Brand> brands = await _memoryCache.GetOrCreateAsync(ConstantsService.BRANDS, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(3);
                return await _brand.GetModels().Where(b => b.Models.Any()).ToListAsync();
            });
            return brands;
        }

        //[HttpGet("{id}")]
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        //public async Task<ContentResult> GetSvg(int id)
        //{
        //    string svg = await F.ReadAllTextAsync($"{_path.GetSVGFolder()}/category/{id}.svg");
        //    return base.Content($"<html style=\"width: fit-content; height: fit-content; margin: 0\"><head><meta name\"viewport\" content=\"width=fit-content, initial-scale=1.0\" ></head><body style=\"width: fit-content; height: fit-content; margin: 0\">{svg}</body></html>", "text/html");
        //}
    }
}

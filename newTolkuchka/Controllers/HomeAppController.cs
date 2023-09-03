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
    [Route("api/[controller]/[action]/{id?}")]
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


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<JsonResult> Products([FromQuery] string model, [FromQuery] string id, [FromQuery] bool productsOnly, [FromQuery] int[] t, [FromQuery] int[] b, [FromQuery] string[] v, [FromQuery] int minp, [FromQuery] int maxp, [FromQuery] Sort sort, [FromQuery] int page, [FromQuery] int pp = 60, [FromQuery] string search = null)
        {
            JsonResult result = await ProductsBase(model, id, productsOnly, t, b, v, minp, maxp, sort, page, pp, search, true);
            return result;
        }

        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public async Task<IList<Category>> ChildCategories(int id)
        {
            IList<Category> categories = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORIESCHILDRENBYPARENTID}{id}", async ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                return await _category.GetActiveCategoriesByParentId(id).ToListAsync();
            });
            return categories;
        }
    }
}

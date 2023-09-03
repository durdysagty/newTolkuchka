#region using
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using F = System.IO.File;
#endregion
namespace newTolkuchka.Services.Abstracts
{
    public class AbstractHomeController : Controller
    {
        #region staff
        private protected readonly IStringLocalizer<Shared> _localizer;
        private protected readonly IBreadcrumbs _breadcrumbs;
        private protected readonly IPath _path;
        private protected readonly ICategory _category;
        private protected readonly IBrand _brand;
        private protected readonly IPromotion _promotion;
        private protected readonly IModel _model;
        private protected readonly IProduct _product;
        private protected readonly ISlide _slide;
        private protected readonly IActionNoFile<Heading, Heading> _heading;
        private protected readonly IArticle _article;
        private protected readonly IUser _user;
        private protected readonly ICustomerGuid _customerGuid;
        private protected readonly IInvoice _invoice;
        private protected readonly IOrder _order;
        private protected readonly ILogin _login;
        private protected readonly IActionNoFile<Currency, AdminCurrency> _currency;
        private protected readonly IMemoryCache _memoryCache;
        private protected readonly ICrypto _crypto;
        // to remove
        private protected readonly ILogger<AbstractHomeController> _logger;

        public AbstractHomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IPromotion promotion, IModel model, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, ICustomerGuid customerGuid, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache, ICrypto crypto, ILogger<AbstractHomeController> logger)
        {
            _localizer = localizer;
            _breadcrumbs = breadcrumbs;
            _path = path;
            _category = category;
            _brand = brand;
            _promotion = promotion;
            _model = model;
            _product = product;
            _slide = slide;
            _heading = heading;
            _article = article;
            _user = user;
            _customerGuid = customerGuid;
            _order = order;
            _invoice = invoice;
            _login = login;
            _currency = currency;
            _memoryCache = memoryCache;
            _crypto = crypto;
            _logger = logger;
        }
        #endregion

        private protected const int selectedCategoryId = 11;
        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexSelectedCatProducts(int count)
        {
            IEnumerable<int> mobileIds = _model.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, selectedCategoryId } }).OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> mobileUIProducts = new();
            foreach (int m in mobileIds)
            {
                Product[] mobileProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => !p.NotInUse).ToArrayAsync();
                mobileUIProducts.Add(_product.GetUIProduct(mobileProducts).ToList());
            }
            return mobileUIProducts;
        }

        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexNewProducts(int count)
        {
            IEnumerable<int> newModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.CategoryId != selectedCategoryId && m.Products.Any(p => p.IsNew && !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> newUIProducts = new();
            foreach (int m in newModelIds)
            {
                Product[] newProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsNew && !p.NotInUse).ToArrayAsync();
                newUIProducts.Add(_product.GetUIProduct(newProducts).ToList());
            }
            return newUIProducts;
        }

        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexRecProducts(int count)
        {
            IEnumerable<int> recModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => p.IsRecommended && !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> recUIProducts = new();
            foreach (int m in recModelIds)
            {
                Product[] recProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsRecommended && !p.NotInUse).ToArrayAsync();
                recUIProducts.Add(_product.GetUIProduct(recProducts).ToList());
            }
            return recUIProducts;
        }


        private protected async Task<JsonResult> ProductsBase(string model, string id, bool productsOnly, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp = 60, string search = null, bool isApp = false)
        {
            string key = $"{CultureProvider.CurrentCulture}{model}{id}{productsOnly}t-{string.Join("", t)}b-{string.Join("", b)}v-{string.Join("", v)}{minp}{maxp}{sort}{page}{search}-{isApp}";
            JsonResult result = await _memoryCache.GetOrCreateAsync(key, async ce =>
            {
                // all caches cleaned when any product changed
                ce.SlidingExpiration = TimeSpan.FromHours(6);
                if (!_memoryCache.TryGetValue(ConstantsService.MODELEDPRODUCTSHASHKEYS, out HashSet<string> modeledProductKeys))
                    modeledProductKeys = new HashSet<string>();
                modeledProductKeys.Add(key);
                _memoryCache.Set(ConstantsService.MODELEDPRODUCTSHASHKEYS, modeledProductKeys, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromDays(3)
                });
                IList<Product> list = new List<Product>();
                IEnumerable<Product> targetProducts = null;
                bool brandsOnly = false;
                bool typesNeeded = false;
                switch (model)
                {
                    case ConstantsService.CATEGORY:
                        // get all products in the category including subcategories
                        // maybe possibility to optimize
                        int categoryId = int.Parse(id);
                        brandsOnly = !_product.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, new List<int> { categoryId } } }).Any();
                        IList<int> categoryIds = await _category.GetAllCategoryIdsHaveProductsByParentIdCachedAsync(categoryId);
                        if (!categoryIds.Any())
                            return new JsonResult(new
                            {
                                products = list,
                                noProduct = _localizer["noProductAbsolutly"].Value
                            });
                        targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, categoryIds } });
                        break;
                    case ConstantsService.BRAND:
                        typesNeeded = true;
                        // get all products in by brand
                        targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.BRAND, new int[] { int.Parse(id) } } });
                        break;
                    case ConstantsService.SEARCH:
                        brandsOnly = true;
                        typesNeeded = true;
                        IList<string> words = search.Trim().Split(" ");
                        targetProducts = await _product.GetFullModels().Where(p => p.Model.Type.NameRu.Contains(words[0]) || p.Model.Type.NameEn.Contains(words[0]) || p.Model.Type.NameTm.Contains(words[0]) || p.Model.Brand.Name.Contains(words[0]) || p.Model.Name.Contains(words[0]) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[0]) || psv.SpecsValue.NameEn.Contains(words[0]) || psv.SpecsValue.NameTm.Contains(words[0])) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[0]) || psvm.SpecsValueMod.NameEn.Contains(words[0]) || psvm.SpecsValueMod.NameTm.Contains(words[0])) || (p.Model.Line != null && p.Model.Line.Name.Contains(words[0]))).ToListAsync();
                        if (targetProducts.Any())
                        {
                            for (var i = 1; i < words.Count; i++)
                            {
                                targetProducts = targetProducts.Where(p => p.Model.Type.NameRu.Contains(words[i], StringComparison.OrdinalIgnoreCase) || p.Model.Type.NameEn.Contains(words[i], StringComparison.OrdinalIgnoreCase) || p.Model.Type.NameTm.Contains(words[i], StringComparison.OrdinalIgnoreCase) || p.Model.Brand.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase) || p.Model.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[i], StringComparison.OrdinalIgnoreCase) || psv.SpecsValue.NameEn.Contains(words[i], StringComparison.OrdinalIgnoreCase) || psv.SpecsValue.NameTm.Contains(words[i], StringComparison.OrdinalIgnoreCase)) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[i], StringComparison.OrdinalIgnoreCase) || psvm.SpecsValueMod.NameEn.Contains(words[i], StringComparison.OrdinalIgnoreCase) || psvm.SpecsValueMod.NameTm.Contains(words[i], StringComparison.OrdinalIgnoreCase)) || (p.Model.Line != null && p.Model.Line.Name.Contains(words[i], StringComparison.OrdinalIgnoreCase))).ToList();
                                if (!targetProducts.Any())
                                    break;
                            }
                        }
                        break;
                    case ConstantsService.NOVELTIES:
                        brandsOnly = true;
                        targetProducts = _product.GetFullModels().Where(p => p.IsNew);
                        break;
                    case ConstantsService.PROMOTION:
                        typesNeeded = true;
                        // get all products by promotion
                        targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, id } });
                        break;
                    case ConstantsService.RECOMMENDED:
                        brandsOnly = true;
                        targetProducts = _product.GetFullModels().Where(p => p.IsRecommended);
                        break;
                    case ConstantsService.LIKED:
                        brandsOnly = true;
                        targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, JsonSerializer.Deserialize<IList<int>>(id) } });
                        break;
                }
                if (targetProducts.Any())
                {
                    list = targetProducts.Where(p => !p.NotInUse && !p.Model.Category.NotInUse).ToList();
                }
                if (!list.Any())
                    return new JsonResult(new
                    {
                        products = list,
                        noProduct = model == ConstantsService.SEARCH ? _localizer["noProductSearch"].Value : _localizer["noProductAbsolutly"].Value
                    });
                IList<IEnumerable<UIProduct>> uiProducts = _product.GetUIData(productsOnly, brandsOnly, typesNeeded, list, t, b, v, minp, maxp, sort, page, pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
                if (isApp)
                {
                    return new JsonResult(new
                    {
                        products = uiProducts,
                        brands = new
                        {
                            name = _localizer[ConstantsService.BRAND].Value,
                            brands
                        },
                        types = new
                        {
                            name = _localizer[ConstantsService.TYPE].Value,
                            types
                        },
                        filters,
                        min,
                        max,
                        sort = new string[]
                        {
                           _localizer["new"].Value,
                           _localizer["price"].Value,
                           _localizer["name"].Value
                        },
                        lastPage,
                        noProduct = uiProducts.Any() ? null : _localizer["noProduct"].Value
                    });
                }
                IEnumerable<string> products = uiProducts.Select(p => IProduct.GetHtmlProduct(p, 12, 6, 6, 4, 4, 3, 3, 2));
                string buttons = $"<i class=\"fas fa-angle-double-left ps-2\" role=\"button\" onclick=\"setPage(0)\" aria-label=\"{_localizer["first"].Value}\"></i><i class=\"fas fa-angle-left ps-1\" role=\"button\" onclick=\"setPage(null, 0)\" aria-label=\"{_localizer["prev"].Value}\"></i><i class=\"fas fa-angle-right ps-1\" role=\"button\" onclick=\"setPage(null, {lastPage})\" aria-label=\"{_localizer["nex"].Value}\"></i><i class=\"fas fa-angle-double-right ps-1\" role=\"button\" onclick=\"setPage({lastPage})\" aria-label=\"{_localizer["last"].Value}\"></i>";
                return new JsonResult(new
                {
                    products,
                    brands = new
                    {
                        name = _localizer[ConstantsService.BRAND].Value,
                        brands
                    },
                    types = new
                    {
                        name = _localizer[ConstantsService.TYPE].Value,
                        types
                    },
                    filters,
                    min,
                    max,
                    sort = new string[]
                    {
                       _localizer["new"].Value,
                       _localizer["price"].Value,
                       _localizer["name"].Value
                    },
                    pagination,
                    buttons,
                    noProduct = uiProducts.Any() ? null : _localizer["noProduct"].Value
                });
            });
            return result;
        }
    }
}
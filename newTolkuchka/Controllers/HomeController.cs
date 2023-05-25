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
namespace newTolkuchka.Controllers
{
    public class HomeController : Controller
    {
        #region staff
        private readonly IStringLocalizer<Shared> _localizer;
        private readonly IBreadcrumbs _breadcrumbs;
        private readonly IPath _path;
        private readonly ICategory _category;
        private readonly IBrand _brand;
        private readonly IPromotion _promotion;
        private readonly IModel _model;
        private readonly IProduct _product;
        private readonly ISlide _slide;
        private readonly IActionNoFile<Heading, Heading> _heading;
        private readonly IArticle _article;
        private readonly IUser _user;
        private readonly ICustomerGuid _customerGuid;
        private readonly IInvoice _invoice;
        private readonly IOrder _order;
        private readonly ILogin _login;
        private readonly IActionNoFile<Currency, AdminCurrency> _currency;
        private readonly IMemoryCache _memoryCache;
        private readonly ICrypto _crypto;
        // to remove
        private readonly ILogger<HomeController> _logger;

        public HomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IPromotion promotion, IModel model, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, ICustomerGuid customerGuid, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache, ICrypto crypto, ILogger<HomeController> logger)
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
        [Route(ConstantsService.SLASH)]
        [Route(ConstantsService.HOME)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 180)]
        public async Task<IActionResult> Index()
        {
            ViewBag.PrCnt = await _memoryCache.GetOrCreateAsync($"{ConstantsService.PRODUCTSCOUNT}", async ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);
                return await _product.GetModels().CountAsync();
            });
            await CreateMetaData();
            return View();
        }
        [Route($"{ConstantsService.INDEX}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<JsonResult> Items()
        {
            #region stuff
            int count = 6;
            int slidesCount = 3;
            string fontSize = ConstantsService.INDEXCATFONTSIZES[3];
            int? width = GetScreenWidth();
            if (width < 351)
                count = 3;
            else if (width < 415)
            {
                slidesCount = 1;
                fontSize = ConstantsService.INDEXCATFONTSIZES[0];
            }
            else if (width < 576)
            {
                slidesCount = 1;
                fontSize = ConstantsService.INDEXCATFONTSIZES[1];
            }
            else if (width < 768)
            {
                slidesCount = 2;
                count = 6;
                fontSize = ConstantsService.INDEXCATFONTSIZES[2];
            }
            else if (width < 992)
            {
                count = 4;
                fontSize = ConstantsService.INDEXCATFONTSIZES[4];
            }
            else if (width < 1200)
            {
                count = 4;
                fontSize = ConstantsService.INDEXCATFONTSIZES[1];
            }
            const int col = 11;
            const int xs = 6;
            const int sm = 4;
            const int md = 3;
            const int lg = 3;
            const int xl = 2;
            const int xxl = 2;
            const int xxxl = 2;
            #endregion
            string brandsString = _memoryCache.GetOrCreate(ConstantsService.HOMEBRANDS, ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IEnumerable<Brand> brands = _brand.GetModels().Where(b => b.IsForHome);
                string brandsString = string.Empty;
                foreach (Brand b in brands)
                {
                    brandsString += $"<div class=\"keen-slider__slide text-center\"><a href=\"/{PathService.GetModelUrl(ConstantsService.BRAND, b.Id)}\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.BRAND, b.Id), b.Version, 180, 60, "100%", "auto", b.Name, "card-img-top rounded border border-primary px-1 py-2")}</a></div>";
                }
                return brandsString;
            });
            string slidesString = _memoryCache.GetOrCreate($"{CultureProvider.CurrentCulture}{ConstantsService.MAINSLIDES}{slidesCount}", ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IEnumerable<Slide> mainSlides = _slide.GetSlidesByLayoutAsync(Layout.Main).OrderByDescending(s => s.Id).Take(slidesCount);
                string slidesString = string.Empty;
                foreach (Slide s in mainSlides)
                {
                    slidesString += $"<div class=\"col-12 col-sm-6 col-md-4 p-1\"><a href=\"{s.Link}\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.SLIDE, s.Id, (int)CultureProvider.CurrentCulture), s.Version, 600, 300, "100%", "auto", $"slide-{s.Id}", "card-img-top rounded")}</a></div>";
                }
                return slidesString;
            });
            static IEnumerable<string> GetHtmlProducts(IList<IEnumerable<UIProduct>> products, int? sw)
            {
                return products.Select(u => IProduct.GetHtmlProduct(u, sw, col, xs, sm, md, lg, xl, xxl, xxxl));
            }
            static string GetItems(IEnumerable<string> strings)
            {
                string products = null;
                foreach (string p in strings)
                {
                    products += p;
                }
                return products;
            }
            int? sw = GetScreenWidth();
            string html = await _memoryCache.GetOrCreateAsync($"{CultureProvider.CurrentCulture}{ConstantsService.INDEXITEMS}{(sw <= ConstantsService.PHONEWIDTH ? ConstantsService.PHONEW : ConstantsService.PCW)}{count}", async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                Category mobileCategory = await _category.GetModelAsync(11);
                IEnumerable<int> mobileIds = _model.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, mobileCategory.Id } }).OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => !p.NotInUse)).Take(count).Select(m => m.Id);
                List<IEnumerable<UIProduct>> mobileUIProducts = new();
                foreach (int m in mobileIds)
                {
                    Product[] mobileProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => !p.NotInUse).ToArrayAsync();
                    mobileUIProducts.Add(_product.GetUIProduct(mobileProducts).ToList());
                }
                IEnumerable<int> newModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.CategoryId != mobileCategory.Id && m.Products.Any(p => p.IsNew && !p.NotInUse)).Take(count).Select(m => m.Id);
                IEnumerable<string> mobileProductsHtml = GetHtmlProducts(mobileUIProducts, sw);
                List<IEnumerable<UIProduct>> newUIProducts = new();
                foreach (int m in newModelIds)
                {
                    Product[] newProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsNew && !p.NotInUse).ToArrayAsync();
                    newUIProducts.Add(_product.GetUIProduct(newProducts).ToList());
                }
                IEnumerable<string> newProductsHtml = GetHtmlProducts(newUIProducts, sw);
                IEnumerable<int> recModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => p.IsRecommended && !p.NotInUse)).Take(count).Select(m => m.Id);
                List<IEnumerable<UIProduct>> recUIProducts = new();
                foreach (int m in recModelIds)
                {
                    Product[] recProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsRecommended && !p.NotInUse).ToArrayAsync();
                    recUIProducts.Add(_product.GetUIProduct(recProducts).ToList());
                }
                IEnumerable<string> recProductsHtml = GetHtmlProducts(recUIProducts, sw);
                string template = "<div class=\"fs-5 px-3 mb-3 border-bottom border-primary\"><a href=\"/{0}\">{1}<span class=\"ms-2\">{2}</span></a></div><div class=\"row justify-content-center\">{3}</div>";
                string html = string.Format(template, PathService.GetModelUrl(ConstantsService.CATEGORY, mobileCategory.Id), F.ReadAllText($"{_path.GetSVGFolder()}/{ConstantsService.CATEGORY}/{mobileCategory.Id}.svg"), CultureProvider.GetLocalName(mobileCategory.NameRu, mobileCategory.NameEn, mobileCategory.NameTm), GetItems(mobileProductsHtml));
                html += string.Format(template, ConstantsService.NOVELTIES, F.ReadAllText($"{_path.GetSVGFolder()}/new.svg"), _localizer[ConstantsService.NOVELTIES].Value, GetItems(newProductsHtml));
                html += string.Format(template, ConstantsService.RECOMMENDED, F.ReadAllText($"{_path.GetSVGFolder()}/rec.svg"), _localizer[ConstantsService.RECOMMENDED].Value, GetItems(recProductsHtml));
                return html;
            });
            string catsString = _memoryCache.GetOrCreate($"{CultureProvider.CurrentCulture}{ConstantsService.INDEXCATS}{fontSize}", ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                string catsTemplate = "<div class=\"col-12 col-xs-6 col-lg-3 p-1\"><a href=\"/{0}\">{1}<div style=\"font-size: {3}rem\" class=\"bg-primary p-1 rounded-bottom\">{2}</div></a></div>";
                string imageClasses = "card-img-top rounded-top";
                IEnumerable<Category> indexCats = _category.GetIndexCategories();
                string catsString = string.Empty;
                foreach (Category c in indexCats)
                {
                    string locaName = CultureProvider.GetLocalName(c.NameRu, c.NameEn, c.NameTm);
                    catsString += string.Format(catsTemplate, PathService.GetModelUrl(ConstantsService.CATEGORY, c.Id), IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.CATEGORY, c.Id), c.Version, 450, 225, "100%", "auto", locaName, imageClasses), locaName, fontSize);
                }
                IEnumerable<Promotion> promotions = _promotion.GetModels().Where(p => !p.NotInUse);
                foreach (Promotion p in promotions)
                {
                    string locaName = CultureProvider.GetLocalName(p.NameRu, p.NameEn, p.NameTm);
                    catsString += string.Format(catsTemplate, PathService.GetModelUrl(ConstantsService.PROMOTION, p.Id), IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.PROMOTION, p.Id), p.Version, 600, 300, "100%", "auto", locaName, imageClasses), locaName, fontSize);
                }
                return catsString;
            });
            string articleStrings = _memoryCache.GetOrCreate($"{CultureProvider.CurrentCulture}{ConstantsService.HOMEARTICLES}{count}", ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                IEnumerable<Article> articles = _article.GetModels(new Dictionary<string, object> { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } }).OrderByDescending(a => a.Id).Take(count);
                if (!articles.Any())
                    return string.Empty;
                string articleStrings = null;
                string arts = string.Empty;
                foreach (Article a in articles)
                {
                    arts += $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id), a.Version, 0, 250, "auto", "125px", a.Name, "rounded-1")}</div><div><small class=\"small text-center\">{a.Name}</small></div></a></div>";
                }
                articleStrings = $"<div class=\"row justify-content-center\">{arts}</div><div class=\"d-flex justify-content-end\"><a href=\"/{ConstantsService.ARTICLES}\"><small>{_localizer["all-articles"].Value}</small></a></div>";
                return articleStrings;
            });
            return new JsonResult(new
            {
                fl = brandsString, // firstLine
                p = slidesString, // slides
                i = html, // items
                c = catsString, // index categories
                a = articleStrings
            });
        }
        [Route(ConstantsService.CATEGORIES)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
        public async Task<IActionResult> Categories()
        {
            IEnumerable<CategoryTree> categories = await _memoryCache.GetOrCreateAsync(ConstantsService.CATEGORIES, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(3);
                return await _category.GetCategoryTree();
            });
            await CreateMetaData(ConstantsService.CATEGORIES, _breadcrumbs.GetBreadcrumbs());
            return View(categories);
        }
        [Route(ConstantsService.BRANDS)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
        public async Task<IActionResult> Brands()
        {
            IList<Brand> brands = await _memoryCache.GetOrCreateAsync(ConstantsService.BRANDS, async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(3);
                return await _brand.GetModels().Where(b => b.Models.Any()).ToListAsync();
            });
            await CreateMetaData(ConstantsService.BRANDS, _breadcrumbs.GetBreadcrumbs());
            return View(brands);
        }
        [Route(ConstantsService.PROMOTIONS)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
        public async Task<IActionResult> Promotions()
        {
            IQueryable<Promotion> promotions = _promotion.GetModels().Where(p => !p.NotInUse);
            await CreateMetaData(ConstantsService.PROMOTIONS, _breadcrumbs.GetBreadcrumbs());
            return View(promotions);
        }
        [Route($"{ConstantsService.CATEGORY}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<IActionResult> Category(int id)
        {
            Category category = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORY}{id}", async ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromHours(6);
                return await _category.GetModelAsync(id);
            });
            if (category == null || category.NotInUse)
                return await GetNotFoundPageAsync();
            string localName = CultureProvider.GetLocalName(category.NameRu, category.NameEn, category.NameTm);
            ViewBag.Categories = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORIESCHILDRENBYPARENTID}{id}", async ce =>
            {
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                return await _category.GetActiveCategoriesByParentId(id).ToListAsync();
            });
            await CreateMetaData(ConstantsService.CATEGORY, await _breadcrumbs.GetCategoryBreadcrumbsAsync(category.ParentId), localName, true, false);
            return View(null, localName);
        }
        [Route($"{ConstantsService.BRAND}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<IActionResult> Brand(int id)
        {
            Brand brand = await _brand.GetModelAsync(id);
            if (brand == null)
                return await GetNotFoundPageAsync();
            await CreateMetaData(ConstantsService.BRAND, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.BRANDS), brand.Name, true);
            return View();
        }
        [Route($"{ConstantsService.PROMOTION}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<IActionResult> Promotion(int id)
        {
            Promotion promotion = await _promotion.GetModelAsync(id);
            if (promotion == null || promotion.NotInUse)
                return await GetNotFoundPageAsync();
            string desc = $"<p class=\"mb-2\">{CultureProvider.GetLocalName(promotion.DescRu, promotion.DescEn, promotion.DescTm)}</p>";
            if (promotion.Type is Tp.ProductFree or Tp.Set or Tp.SpecialSetDiscount)
            {
                Product product = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { (int)promotion.SubjectId } } }).FirstOrDefaultAsync();
                if (product.NotInUse)
                {
                    promotion.NotInUse = true;
                    await _promotion.SaveChangesAsync();
                    return await GetNotFoundPageAsync();
                }
                string name = IProduct.GetProductNameCounted(product);
                string additional = $"<h5><a href=\"/{PathService.GetModelUrl(ConstantsService.PRODUCT, product.Id)}\">{IImage.GetImageHtml(PathService.GetImageRelativePath($"{ConstantsService.PRODUCT}/small", product.Id), product.Version, 200, 200, "50px", "auto", name, "mx-2")}{name}</a> - ";
                desc += additional;
                string additional2 = promotion.Type == Tp.SpecialSetDiscount ? CultureProvider.GetLocalName($"СКИДКА {(int)promotion.Volume}%", $"{(int)promotion.Volume}% OFF", $"ARZANLADYŞ {(int)promotion.Volume}") : $"{_localizer["forfree"]}!</h5>";
                desc += additional2;
            }
            if (promotion.Type is Tp.Set or Tp.SetDiscount or Tp.SpecialSetDiscount)
            {
                ICollection<Product> set = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, promotion.Id } }).ToArrayAsync();
                if (set.Any(s => s.NotInUse))
                {
                    promotion.NotInUse = true;
                    await _promotion.SaveChangesAsync();
                    return await GetNotFoundPageAsync();
                }
            }
            ViewBag.Desc = desc;
            if (promotion == null)
                return await GetNotFoundPageAsync();
            await CreateMetaData(ConstantsService.PROMOTION, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.PROMOTIONS), CultureProvider.GetLocalName(promotion.NameRu, promotion.NameEn, promotion.NameTm), true);
            return View();
        }
        [Route(ConstantsService.SEARCH)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrEmpty(search))
                return RedirectToAction("");
            await CreateMetaData(ConstantsService.SEARCH, _breadcrumbs.GetBreadcrumbs(), _localizer["search"].Value, true);
            return View();
        }
        //[Route($"{{special}}")]
        [Route(ConstantsService.NOVELTIES)]
        [Route(ConstantsService.RECOMMENDED)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 43200)]
        public async Task<IActionResult> Special()
        {
            await CreateMetaData(CultureProvider.Path.Remove(0, 1), _breadcrumbs.GetBreadcrumbs(), null, true);
            return View();
        }
        [Route(ConstantsService.LIKED)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<IActionResult> Liked()
        {
            await CreateMetaData(ConstantsService.LIKED, _breadcrumbs.GetBreadcrumbs(), null, true);
            return View();
        }
        [Route($"{ConstantsService.PRODUCTS}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Products(string model, string id, bool productsOnly, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp = 60, string search = null)
        {
            if (string.IsNullOrEmpty(model))
                return await GetNotFoundPageAsync();
            int? sw = GetScreenWidth();
            string key = $"{CultureProvider.CurrentCulture}{model}{id}{(sw <= ConstantsService.PHONEWIDTH ? ConstantsService.PHONEW : ConstantsService.PCW)}{productsOnly}t-{string.Join("", t)}b-{string.Join("", b)}v-{string.Join("", v)}{minp}{maxp}{sort}{page}{search}";
            JsonResult result = await _memoryCache.GetOrCreateAsync(key, async ce =>
            {
                // all caches cleaned on any product changed
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
                IEnumerable<string> products = uiProducts.Select(p => IProduct.GetHtmlProduct(p, sw, 12, 6, 6, 4, 4, 3, 3, 2));
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
        [Route($"{ConstantsService.PRODUCT}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10800)]
        public async Task<IActionResult> Product(int id)
        {
            int? sw = GetScreenWidth();
            string key = $"{CultureProvider.CurrentCulture}{ConstantsService.PRODUCTKEY}-{id}-{(sw > ConstantsService.MOBILEWIDTH ? sw > ConstantsService.XLWIDTH ? ConstantsService.XLCW : ConstantsService.PCW : ConstantsService.MOBILEW)}";
            ViewResult viewResult = await _memoryCache.GetOrCreateAsync(key, async ce =>
            {
                // removed in put & delete product, model
                // removed on put type, specsValue, specsValueMod
                ce.SlidingExpiration = TimeSpan.FromMinutes(10);
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                if (!_memoryCache.TryGetValue(ConstantsService.PRODUCTSHASHKEYS, out HashSet<string> productKeys))
                    productKeys = new HashSet<string>();
                productKeys.Add(key);
                _memoryCache.Set(ConstantsService.PRODUCTSHASHKEYS, productKeys, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromDays(3)
                });
                Product product = await _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { id } } }).Include(p => p.Model).ThenInclude(m => m.Category).FirstOrDefaultAsync();
                if (product == null)
                    return (ViewResult)await GetNotFoundPageAsync();
                if (product.NotInUse || product.Model.Category.NotInUse)
                {
                    product = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { id } } }).FirstOrDefaultAsync();
                    string localName = IProduct.GetProductNameCounted(product);
                    await CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
                    return View();
                }
                IList<Product> products = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, product.ModelId } }).Where(p => !p.NotInUse).ToListAsync();
                IList<(string, Product, IList<Promo>)> namedProducts = new List<(string, Product, IList<Promo>)>();
                // to select Specs from any of product for change products by specs value
                IList<ModelWithList<ModelWithList<AdminSpecsValueMod>>> nameUsedSpecs = products.FirstOrDefault().Model.ModelSpecs.Where(s => s.IsNameUse).OrderBy(s => s.Spec.NamingOrder).Select(ms => new ModelWithList<ModelWithList<AdminSpecsValueMod>>
                {
                    Id = ms.Spec.Id,
                    Name = CultureProvider.GetLocalName(ms.Spec.NameRu, ms.Spec.NameEn, ms.Spec.NameTm),
                    Is = ms.Spec.IsImaged,
                    List = new Collection<ModelWithList<AdminSpecsValueMod>>()
                }).ToList();
                // ids of namedSpecs we need to select specsValues, which specId is in namedSpecIds, from products and we need them in Product page
                IEnumerable<int> namedSpecIds = nameUsedSpecs.Select(ms => ms.Id);
                foreach (Product p in products)
                {
                    // preparing Promos of Product
                    List<Promo> promos = new();
                    foreach (Promotion promotion in p.PromotionProducts.Select(pp => pp.Promotion))
                    {
                        Promo promo = new()
                        {
                            Id = promotion.Id,
                            Type = promotion.Type,
                            Volume = promotion.Volume,
                            Quantity = promotion.Quantity,
                            Name = CultureProvider.GetLocalName(promotion.NameRu, promotion.NameEn, promotion.NameTm)
                        };
                        if (promotion.Type is Tp.ProductFree or Tp.Set or Tp.SpecialSetDiscount)
                        {
                            Product subject = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { (int)promotion.SubjectId } } }).FirstOrDefaultAsync();
                            // check is the subject active, if not promotion should to be not active also
                            if (subject.NotInUse)
                            {
                                promotion.NotInUse = true;
                                await _product.SaveChangesAsync();
                                continue;
                            }
                            promo.Subject = subject;
                        }
                        if (promotion.Type is Tp.Set or Tp.SetDiscount or Tp.SpecialSetDiscount)
                        {
                            ICollection<Product> set = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, promo.Id } }).ToArrayAsync();
                            if (set.Any(s => s.NotInUse))
                            {
                                promotion.NotInUse = true;
                                await _product.SaveChangesAsync();
                                continue;
                            }
                            promo.Products = set;
                        }
                        promos.Add(promo);
                    }
                    // selecting of named specsValues from the product
                    IEnumerable<SpecsValue> psvs = p.ProductSpecsValues.Select(psv => psv.SpecsValue).Where(x => namedSpecIds.Contains(x.SpecId));
                    string name = IProduct.GetProductNameSingle(p, psvs);
                    namedProducts.Add((name, p, promos));
                    // loop the specsValues to add them to nameUsedSpecs unique
                    foreach (SpecsValue sv in psvs)
                    {
                        ModelWithList<ModelWithList<AdminSpecsValueMod>> spec = nameUsedSpecs.FirstOrDefault(s => s.Id == sv.SpecId);
                        if (!spec.List.Any(s => s.Id == sv.Id))
                        {
                            ModelWithList<AdminSpecsValueMod> specValue = new()
                            {
                                Id = sv.Id,
                                Name = CultureProvider.GetLocalName(sv.NameRu, sv.NameEn, sv.NameTm),
                                List = new Collection<AdminSpecsValueMod>(),
                                Version = sv.Version
                            };
                            spec.List.Add(specValue);
                        }
                        // check if a product has a specsValuMod (modification of specsValue)
                        SpecsValueMod svm = p.ProductSpecsValueMods.FirstOrDefault(x => x.SpecsValueMod.SpecsValueId == sv.Id)?.SpecsValueMod;
                        // if svm is null, that mean a product do not have a modification od this specsValue
                        if (svm == null && !spec.List.FirstOrDefault(s => s.Id == sv.Id).List.Any(asvm => asvm.Id == 0))
                        {
                            // dispite of null we need to add 0 Id specsValueMod, to select Product without modificated specsValue in Product page
                            spec.List.FirstOrDefault(s => s.Id == sv.Id).List.Add(new AdminSpecsValueMod
                            {
                                Id = 0
                            });
                        }
                        else if (svm != null && !spec.List.FirstOrDefault(s => s.Id == sv.Id).List.Any(asvm => asvm.Id == svm.Id))
                        {
                            // unique adding of specsValueMod to List of specsValueMods of specsValue of Spec
                            AdminSpecsValueMod adminSpecsValueMod = new()
                            {
                                Id = svm.Id,
                                Name = CultureProvider.GetLocalName(svm.NameRu, svm.NameEn, svm.NameTm)
                            };
                            spec.List.FirstOrDefault(s => s.Id == sv.Id).List.Add(adminSpecsValueMod);
                        }
                    }
                }
                await CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), namedProducts.FirstOrDefault(x => x.Item2.Id == id).Item1, false, false);
                ViewBag.Specs = nameUsedSpecs;
                ViewBag.SpecIds = namedSpecIds;
                ViewBag.Current = product.Id;
                return View(namedProducts);
            });
            return viewResult;
        }
        [Route(ConstantsService.COMPARISON)]
        public async Task<IActionResult> Comparison()
        {
            bool isScaled = HttpContext.Request.Cookies.TryGetValue("scaled", out string scaled);
            await CreateMetaData(ConstantsService.COMPARISON, _breadcrumbs.GetBreadcrumbs());
            if (!isScaled)
                return View();
            int[] ids = JsonSerializer.Deserialize<int[]>(scaled.Replace("-", ","));
            List<(string, Product, decimal)> coms = new();
            Product[] products = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, ids } }).ToArrayAsync();
            List<Spec> specs = new();
            foreach (Product p in products)
            {
                string name = IProduct.GetProductNameCounted(p);
                foreach (Spec psv in p.ProductSpecsValues.Select(psv => psv.SpecsValue.Spec))
                {
                    if (!specs.Any(s => s.Id == psv.Id))
                    {
                        specs.Add(psv);
                    }
                }
                Promotion discount = p.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Discount)?.Promotion;
                decimal price = discount != null ? IProduct.GetConvertedPrice((decimal)(p.Price - p.Price * discount.Volume / 100)) : p.NewPrice != null ? IProduct.GetConvertedPrice((decimal)p.NewPrice) : IProduct.GetConvertedPrice(p.Price);
                coms.Add((name, p, price));
            }
            ViewBag.Specs = specs;
            return View(coms);
        }
        [Route($"{ConstantsService.ARTICLES}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10000)]
        public async Task<IActionResult> Articles()
        {
            IEnumerable<Heading> headings = _heading.GetModels(new Dictionary<string, object> { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } });
            await CreateMetaData(ConstantsService.ARTICLES, _breadcrumbs.GetBreadcrumbs());
            return View(headings);
        }
        [Route($"{ConstantsService.ARTICLES}bh")] // (bh for by headings)
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10000)]
        public JsonResult Articles(int? headingId, int page)
        {
            int? width = GetScreenWidth();
            int pp = width switch
            {
                < 576 => 20,
                < 992 => 40,
                < 1400 => 60,
                _ => 80,
            };
            Dictionary<string, object> parameters = new() { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } };
            if (headingId != null)
            {
                parameters.Add(ConstantsService.HEADING, headingId);
            }
            int toSkip = page * pp;
            IEnumerable<Article> preArticles = _article.GetModels(parameters);
            int total = preArticles.Count();
            preArticles = preArticles.OrderByDescending(a => a.Id).Skip(toSkip).Take(pp);
            const int col = 12;
            //const int xs = 6;
            const int sm = 6;
            //const int md = 3;
            const int lg = 4;
            //const int xl = 2;
            const int xxl = 3;
            //const int xxxl = 3;
            string articles = string.Empty;
            foreach (Article a in preArticles)
            {
                articles += $"<div class=\"col-{col} col-sm-{sm} col-lg-{lg} col-xxl-{xxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id), a.Version, 0, 250, "auto", "120px", a.Name, "rounded-1")}</div><p>{a.Name}</p><div class=\"text-end\"><small>{a.Date.ToShortDateString()}</small></div></a></div>";
            }
            string pagination = _article.GetPagination(pp, total, preArticles.Count(), toSkip, out int lp);
            int lastPage = lp;
            return new JsonResult(new
            {
                articles,
                pagination,
                lastPage,
                noArticle = articles != string.Empty ? null : _localizer["noArticle"].Value
            });
        }
        [Route($"{ConstantsService.ARTICLE}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 518400)]
        public async Task<IActionResult> Article(int id)
        {
            Article article = await _article.GetModels(new Dictionary<string, object>() { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } }).FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
                return await GetNotFoundPageAsync();
            await CreateMetaData(ConstantsService.ARTICLE, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.ARTICLES), article.Name);
            //ViewBag.ScreenWidth = GetScreenWidth() - 20;
            return View(article);
        }
        [Route(ConstantsService.ABOUT)]
        [Route($"{ConstantsService.DELIVERY}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 259200)]
        public async Task<IActionResult> Content()
        {
            string p = CultureProvider.Path.TrimStart(new char[] { '/' });
            string content = null;
            switch (p)
            {
                case ConstantsService.ABOUT:
                    await CreateMetaData(ConstantsService.ABOUT, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.ABOUT].Value, false);
                    content = await F.ReadAllTextAsync(_path.GetHtmlAboutBodyPath(CultureProvider.Lang));
                    break;
                case ConstantsService.DELIVERY:
                    await CreateMetaData(ConstantsService.DELIVERY, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.DELIVERY].Value, false);
                    content = await F.ReadAllTextAsync(_path.GetHtmlDeliveryBodyPath(CultureProvider.Lang));
                    break;
            }
            return View(null, content);
        }
        [Route($"{ConstantsService.CART}")]
        public async Task<IActionResult> Cart()
        {
            await CreateMetaData(ConstantsService.CART, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.CART].Value);
            User user = await _user.GetCurrentUser();
            if (user != null)
                ViewBag.DeliveryData = new DeliveryData
                {
                    Name = user.Name,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = user.Address
                };
            return View();
        }
        [Route($"{ConstantsService.CART}/data"), HttpPost]
        public JsonResult CartData([FromBody] IList<CartOrder> orders)
        {
            if (orders == null || !orders.Any())
                return new JsonResult(new
                {
                    noOrders = true
                });
            _order.CreateCartOrders(orders);
            if (!orders.Any())
                return new JsonResult(new
                {
                    noOrders = true
                });
            return new JsonResult(new
            {
                orders,
                deliveryFree = ConstantsService.DELIVERYFREE,
                deliveryPrice = ConstantsService.DELIVERYPRICE
            });
        }
        [Route($"{ConstantsService.ORDER}"), HttpPost]
        public async Task<IActionResult> Order([FromForm] string orders, [FromForm] DeliveryData deliveryData)
        {
            IPAddress remoteIp = HttpContext.Connection.RemoteIpAddress;
            StringValues agent = HttpContext.Request.Headers["User-Agent"];
            string blockKey = $"{remoteIp}-{agent}";
            if (_memoryCache.TryGetValue(blockKey, out bool _))
                return BadRequest();
            string u = HttpContext.Request.Cookies[Secrets.userUniqCookie];
            if (string.IsNullOrEmpty(u))
                return BadRequest();
            string test = _crypto.GetUserUniqCookie(u);
            string[] parts = test.Split('@');
            // if uniqUser Guid try to access often than 20 second more than 4 times bolck by ip and agent
            int count = _memoryCache.GetOrCreate(parts[1], ce =>
            {
                ce.Priority = CacheItemPriority.Low;
                ce.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                return 0;
            });
            count++;
            if (count > 4)
            {
                _memoryCache.Set(blockKey, true, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }
            _memoryCache.Set(parts[1], count, new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Low,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
            });
            if (parts[0] != Secrets.uniqCookieTest)
                return BadRequest();
            DateTimeOffset timeOfCookie = DateTimeOffset.Parse(parts[2]);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (now - timeOfCookie < TimeSpan.FromMinutes(Secrets.timeBeforeOrder))
                return BadRequest();
            CartOrder[] cartOrders = JsonService.Deserialize<CartOrder[]>(orders);
            if (cartOrders == null || !cartOrders.Any())
                return new JsonResult(new
                {
                    noOrders = true
                });
            Guid guid = Guid.Parse(parts[1]);
            CustomerGuid customerGuid = await _customerGuid.GetModelAsync(guid);
            if (customerGuid == null)
            {
                customerGuid = new()
                {
                    Id = guid
                };
                await _customerGuid.AddModelAsync(customerGuid);
            }
            if (customerGuid.IsBanned)
                // actually no order will be prosessed but no information will be given to the joker
                return new JsonResult(new
                {
                    success = _localizer["order-success"].Value
                });
            // if user by guid have 3 or more not paid invoices warning
            Invoice[] notPaidInvoices = await _invoice.GetModels(new Dictionary<string, object> { { ConstantsService.CUSTOMERGUID, parts[1] } }).Where(i => i.IsPaid == false).ToArrayAsync();
            if (notPaidInvoices.Length > 2)
                return new JsonResult(new
                {
                    success = _localizer["not-paid-orders"].Value
                });
            await _invoice.CreateInvoice(_user.GetCurrentUser().Result?.Id, cartOrders, deliveryData, customerGuid.Id);
            return new JsonResult(new
            {
                success = _localizer["order-success"].Value
            });
        }
        // no cache, when logout redirect 302 code are cached
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 120)]
        [Route($"{ConstantsService.ACCOUNT}")]
        public async Task<IActionResult> Account()
        {
            User user = await _user.GetCurrentUser();
            if (user == null)
                return RedirectToAction("Index");
            return await AccountGeneric(user);
        }
        [Route($"{ConstantsService.ACCOUNT}"), HttpPost]
        public async Task<IActionResult> Account(AccountUser accountUser)
        {
            User user = await _user.GetCurrentUser();
            if (user == null)
                return RedirectToAction("Index");
            if (accountUser != null)
            {
                bool isPinChanged = await _user.EditUserAsync(accountUser, user);
                if (isPinChanged)
                {
                    HttpContext.Response.Cookies.Delete(Secrets.userTokenCookie);
                    HttpContext.Response.Cookies.Delete(Secrets.userHashCookie);
                    _memoryCache.Remove(ConstantsService.UserHashKey(user.Id));
                    return RedirectToAction(ConstantsService.INDEX);
                }
            }
            return await AccountGeneric(user);
        }
        [Route($"{ConstantsService.N404}")]
        public async Task<IActionResult> NotFoundPage()
        {
            await CreateMetaData();
            return View();
        }
        [HttpGet("recovery/newpin/{guid}")]
        public async Task<IActionResult> PinRecovery(Guid guid)
        {
            LoginResponse loginResponse = await _login.NewPINAsync(guid);
            if (loginResponse.Result == LoginResponse.R.Error)
                return await GetNotFoundPageAsync();
            await CreateMetaData();
            return View(loginResponse);
        }
        private async Task<IActionResult> GetNotFoundPageAsync()
        {
            await CreateMetaData();
            return View("NotFoundPage");
        }
        private async Task<IActionResult> AccountGeneric(User user)
        {
            await CreateMetaData(ConstantsService.ACCOUNT, _breadcrumbs.GetBreadcrumbs(), user.Name);
            ViewBag.Invoices = await _invoice.GetUserInvoicesAsync(user.Id);
            return View(user);
        }
        private async Task CreateMetaData(string pageName = null, IList<Breadcrumb> breadcrumbs = null, string modelName = null, bool filterScript = false, bool isPageName = true)
        {
            IList<Category> mainCategories = await _memoryCache.GetOrCreateAsync($"{ConstantsService.CATEGORIESGROUPBYPARENTID}{0}", async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromDays(10);
                return await _category.GetActiveCategoriesByParentId(0).ToListAsync();
            });
            ViewBag.MainCategories = mainCategories;
            ViewData[ConstantsService.TITLE] = modelName != null ? $"{modelName} - " : pageName != null ? $"{_localizer[pageName]} - " : null;
            ViewData[ConstantsService.DESCRIPTION] = string.Format(_localizer[$"desc-{pageName ?? "home"}"], CultureProvider.SiteName, pageName == ConstantsService.CATEGORY ? modelName.ToLower() : modelName);
            ViewData[ConstantsService.IMAGE] = _path.GetLogo();
            if (isPageName)
                ViewData["PageName"] = modelName != null ? $"{modelName}" : pageName != null ? $"{_localizer[pageName]}" : null;
            ViewBag.Breadcrumbs = breadcrumbs ?? breadcrumbs;
            ViewBag.FilterScript = filterScript;
            int? width = GetScreenWidth();
            if (width != null)
            {
                ViewBag.WindowWidth = width;
                if (ViewBag.MainCategories == null && width < ConstantsService.MOBILEWIDTH)
                    ViewBag.MainCategories = _category.GetActiveCategoriesByParentId(0);
            }
            else
            {
                if (ViewBag.MainCategories == null)
                    ViewBag.MainCategories = _category.GetActiveCategoriesByParentId(0);
            }
        }
        private int? GetScreenWidth()
        {
            bool isWidthSet = HttpContext.Request.Cookies.TryGetValue("w", out string width);
            if (isWidthSet)
                return int.Parse(width);
            else
                return null;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // only for test would be ideal if hide in release
        //[Route("testus/{id}/{count}")]
        //public async Task<IActionResult> Test(int id, int count)
        //{
        //    Stopwatch stopwatch = new();
        //    async Task<double> Method1(int id)
        //    {
        //        stopwatch.Start();
        //        Product product = await _product.GetModelAsync(id);
        //        if (product == null)
        //        {
        //            stopwatch.Stop();
        //            return stopwatch.Elapsed.TotalMilliseconds;
        //        }
        //        IQueryable<Product> products = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, product.ModelId } }).Where(p => !p.NotInUse);
        //        product = products.FirstOrDefault(p => p.Id == id);
        //        string localName = IProduct.GetProductNameCounted(product);
        //        await CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
        //        if (product.NotInUse || product.Model.Category.NotInUse)
        //        {
        //            stopwatch.Stop();
        //            return stopwatch.Elapsed.TotalMilliseconds;
        //        }
        //        if (!products.Any())
        //        {
        //            stopwatch.Stop();
        //            return stopwatch.Elapsed.TotalMilliseconds;
        //        }
        //        // to select Specs from any of product for change products by specs value
        //        IList<ModelWithList<SpecsValue>> nameUsedSpecs = product.Model.ModelSpecs.Where(s => s.IsNameUse).OrderBy(s => s.Spec.NamingOrder).Select(ms => new ModelWithList<SpecsValue>
        //        {
        //            Id = ms.Spec.Id,
        //            Name = CultureProvider.GetLocalName(ms.Spec.NameRu, ms.Spec.NameEn, ms.Spec.NameTm),
        //            Is = ms.Spec.IsImaged,
        //            List = new Collection<SpecsValue>()
        //        }).ToList();
        //        IEnumerable<int> namedSpecIds = nameUsedSpecs.Select(ms => ms.Id);
        //        foreach (Product p in products)
        //        {
        //            foreach (SpecsValue sv in p.ProductSpecsValues.Select(psv => psv.SpecsValue).Where(x => namedSpecIds.Contains(x.SpecId)))
        //            {
        //                ModelWithList<SpecsValue> spec = nameUsedSpecs.FirstOrDefault(s => s.Id == sv.SpecId);
        //                if (!spec.List.Any(s => s.Id == sv.Id))
        //                    spec.List.Add(sv);
        //            }
        //        }
        //        ViewBag.Specs = nameUsedSpecs;
        //        ViewBag.SpecIds = namedSpecIds;
        //        ViewBag.Current = product.Id;
        //        stopwatch.Stop();
        //        return stopwatch.Elapsed.TotalMilliseconds;
        //    }
        //    async Task<double> Method2(int id)
        //    {
        //        stopwatch.Start();
        //        Product product = await _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { id } } }).Include(p => p.Model).ThenInclude(m => m.Category).FirstOrDefaultAsync();
        //        if (product == null)
        //        {
        //            stopwatch.Stop();
        //            return stopwatch.Elapsed.TotalMilliseconds;
        //        }
        //        if (product.NotInUse || product.Model.Category.NotInUse)
        //        {
        //            string localName = IProduct.GetProductNameCounted(product);
        //            await CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
        //            stopwatch.Stop();
        //            return stopwatch.Elapsed.TotalMilliseconds;
        //        }
        //        IList<Product> products = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, product.ModelId } }).Where(p => !p.NotInUse).ToListAsync();
        //        // to select Specs from any of product for change products by specs value
        //        IList<ModelWithList<SpecsValue>> nameUsedSpecs = products.FirstOrDefault().Model.ModelSpecs.Where(s => s.IsNameUse).OrderBy(s => s.Spec.NamingOrder).Select(ms => new ModelWithList<SpecsValue>
        //        {
        //            Id = ms.Spec.Id,
        //            Name = CultureProvider.GetLocalName(ms.Spec.NameRu, ms.Spec.NameEn, ms.Spec.NameTm),
        //            Is = ms.Spec.IsImaged,
        //            List = new Collection<SpecsValue>()
        //        }).ToList();
        //        IEnumerable<int> namedSpecIds = nameUsedSpecs.Select(ms => ms.Id);
        //        foreach (Product p in products)
        //        {
        //            foreach (SpecsValue sv in p.ProductSpecsValues.Select(psv => psv.SpecsValue).Where(x => namedSpecIds.Contains(x.SpecId)))
        //            {
        //                ModelWithList<SpecsValue> spec = nameUsedSpecs.FirstOrDefault(s => s.Id == sv.SpecId);
        //                if (!spec.List.Any(s => s.Id == sv.Id))
        //                    spec.List.Add(sv);
        //            }
        //        }
        //        ViewBag.Specs = nameUsedSpecs;
        //        ViewBag.SpecIds = namedSpecIds;
        //        ViewBag.Current = product.Id;
        //        string localName2 = IProduct.GetProductNameCounted(product);
        //        await CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName2, false, false);
        //        stopwatch.Stop();
        //        return stopwatch.Elapsed.TotalMilliseconds;
        //    }
        //    IList<double> method1 = new List<double>();
        //    IList<double> method2 = new List<double>();
        //    for (int i = 0; i < count; i++)
        //    {
        //        //double result1 = await Method1(id);
        //        //method1.Add(result1);
        //        double result2 = await Method2(id);
        //        method2.Add(result2);
        //        //if (i % 2 == 0)
        //        //{
        //        //    double result1 = await Method1(id);
        //        //    method1.Add(result1);
        //        //}
        //        //else
        //        //{
        //        //    double result2 = await Method2(id);
        //        //    method2.Add(result2);
        //        //}
        //    }
        //    //for (int i = 0; i < count; i++)
        //    //{
        //    //    if (i % 2 != 0)
        //    //    {
        //    //        double result1 = await Method1(id);
        //    //        method1.Add(result1);
        //    //    }
        //    //    else
        //    //    {
        //    //        double result2 = await Method2(id);
        //    //        method2.Add(result2);
        //    //    }
        //    //}
        //    double average1 = method1.Sum(x => x) / method1.Count;
        //    double average2 = method2.Sum(x => x) / method2.Count;
        //    List<string> strings = new()
        //    {
        //        //$"The reslut of method 1 - [{string.Join(" | ", method1)}]",
        //        //$"Average is {average1}",
        //        $"The reslut of method 2 - [{string.Join(" | ", method2)}]",
        //        $"Average is {average2}"
        //};
        //    return View(strings);
        //}
    }
}
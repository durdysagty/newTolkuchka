#region using
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly IInvoice _invoice;
        private readonly IOrder _order;
        private readonly ILogin _login;
        private readonly IActionNoFile<Currency, AdminCurrency> _currency;
        private readonly IMemoryCache _memoryCache;

        public HomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IPromotion promotion, IModel model, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache)
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
            _order = order;
            _invoice = invoice;
            _login = login;
            _currency = currency;
            _memoryCache = memoryCache;
        }
        #endregion
        [Route(ConstantsService.SLASH)]
        [Route(ConstantsService.HOME)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10800)]
        public async Task<IActionResult> Index()
        {
            ViewBag.PrCnt = await _product.GetModels().CountAsync();
            CreateMetaData();
            return View();
        }
        [Route($"{ConstantsService.INDEX}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
        public async Task<JsonResult> Items()
        {
            #region stuff
            int count = 6;
            int slidesCount = 3;
            string fontSize = "1";
            int? width = GetScreenWidth();
            if (width < 351)
                count = 3;
            else if (width < 415)
            {
                slidesCount = 1;
                fontSize = "0.55";
            }
            else if (width < 576)
            {
                slidesCount = 1;
                fontSize = "0.7";
            }
            else if (width < 768)
            {
                slidesCount = 2;
                count = 6;
                fontSize = "0.8";
            }
            else if (width < 992)
            {
                count = 4;
                fontSize = "1.2";
            }
            else if (width < 1200)
            {
                count = 4;
                fontSize = "0.7";
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
            IEnumerable<Brand> brands = _brand.GetModels().Where(b => b.IsForHome);
            string brandsString = string.Empty;
            foreach (Brand b in brands)
            {
                brandsString += $"<div class=\"keen-slider__slide text-center\"><a href=\"/{PathService.GetModelUrl(ConstantsService.BRAND, b.Id)}\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.BRAND, b.Id), 180, 60, "100%", "auto", b.Name, "card-img-top rounded border border-primary px-1 py-2")}</a></div>";
            }
            IEnumerable<Slide> mainSlides = _slide.GetSlidesByLayoutAsync(Layout.Main).OrderByDescending(s => s.Id).Take(slidesCount);
            string slidesString = string.Empty;
            foreach (Slide s in mainSlides)
            {
                slidesString += $"<div class=\"col-12 col-sm-6 col-md-4 p-1\"><a href=\"/{s.Link}\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.SLIDE, s.Id), 600, 300, "100%", "auto", s.Name, "card-img-top rounded")}</a></div>";
            }
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
            IEnumerable<int> newModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => p.IsNew && !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> newUIProducts = new();
            foreach (int m in newModelIds)
            {
                Product[] newProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsNew && !p.NotInUse).ToArrayAsync();
                newUIProducts.Add(_product.GetUIProduct(newProducts).ToList());
            }
            int? sw = GetScreenWidth();
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
            string html = string.Format(template, ConstantsService.NOVELTIES, F.ReadAllText($"{_path.GetSVGFolder()}/new.svg"), _localizer[ConstantsService.NOVELTIES].Value, GetItems(newProductsHtml));
            html += string.Format(template, ConstantsService.RECOMMENDED, F.ReadAllText($"{_path.GetSVGFolder()}/rec.svg"), _localizer[ConstantsService.RECOMMENDED].Value, GetItems(recProductsHtml));
            IEnumerable<Category> indexCats = _category.GetIndexCategories();
            IEnumerable<Promotion> promotions = _promotion.GetModels().Where(p => !p.NotInUse);
            string catsString = string.Empty;
            string catsTemplate = "<div class=\"col-12 col-xs-6 col-lg-3 p-1\"><a href=\"/{0}\">{1}<div style=\"font-size: {3}rem\" class=\"bg-primary p-1 rounded-bottom\">{2}</div></a></div>";
            string imageClasses = "card-img-top rounded-top";
            foreach (Category c in indexCats)
            {
                string locaName = CultureProvider.GetLocalName(c.NameRu, c.NameEn, c.NameTm);
                catsString += string.Format(catsTemplate, PathService.GetModelUrl(ConstantsService.CATEGORY, c.Id), IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.CATEGORY, c.Id), 450, 225, "100%", "auto", locaName, imageClasses), locaName, fontSize);
            }
            foreach (Promotion p in promotions)
            {
                string locaName = CultureProvider.GetLocalName(p.NameRu, p.NameEn, p.NameTm);
                catsString += string.Format(catsTemplate, PathService.GetModelUrl(ConstantsService.PROMOTION, p.Id), IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.PROMOTION, p.Id), 600, 300, "100%", "auto", locaName, imageClasses), locaName, fontSize);
            }
            IEnumerable<Article> articles = _article.GetModels(new Dictionary<string, object> { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } }).OrderByDescending(a => a.Id).Take(count);
            string articleStrings = null;
            if (articles.Count() >= count)
            {
                string arts = string.Empty;
                foreach (Article a in articles)
                {
                    arts += $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id), 0, 250, "auto", "125px", a.Name, "rounded-1")}</div><div><small class=\"small text-center\">{a.Name}</small></div></a></div>";
                }
                articleStrings = $"<div class=\"row justify-content-center\">{arts}</div><div class=\"d-flex justify-content-end\"><a href=\"/{ConstantsService.ARTICLES}\"><small>{_localizer["all-articles"].Value}</small></a></div>";
            }
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public async Task<IActionResult> Categories()
        {
            IEnumerable<CategoryTree> categories = await _category.GetCategoryTree();
            CreateMetaData(ConstantsService.CATEGORIES, _breadcrumbs.GetBreadcrumbs());
            return View(categories);
        }
        [Route(ConstantsService.BRANDS)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public IActionResult Brands()
        {
            IQueryable<Brand> brands = _brand.GetModels().Where(b => b.Models.Any());
            CreateMetaData(ConstantsService.BRANDS, _breadcrumbs.GetBreadcrumbs());
            return View(brands);
        }
        [Route(ConstantsService.PROMOTIONS)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 172800)]
        public IActionResult Promotions()
        {
            IQueryable<Promotion> promotions = _promotion.GetModels().Where(p => !p.NotInUse);
            CreateMetaData(ConstantsService.PROMOTIONS, _breadcrumbs.GetBreadcrumbs());
            return View(promotions);
        }
        [Route($"{ConstantsService.CATEGORY}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public async Task<IActionResult> Category(int id)
        {
            Category category = await _category.GetModelAsync(id);
            if (category == null || category.NotInUse)
                return GetNotFoundPage();
            string localName = CultureProvider.GetLocalName(category.NameRu, category.NameEn, category.NameTm);
            ViewBag.Categories = _category.GetActiveCategoriesByParentId(id);
            CreateMetaData(ConstantsService.CATEGORY, await _breadcrumbs.GetCategoryBreadcrumbsAsync(category.ParentId), localName, true);
            return View();
        }
        [Route($"{ConstantsService.BRAND}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public async Task<IActionResult> Brand(int id)
        {
            Brand brand = await _brand.GetModelAsync(id);
            if (brand == null)
                return GetNotFoundPage();
            CreateMetaData(ConstantsService.BRAND, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.BRANDS), brand.Name, true);
            return View();
        }
        [Route($"{ConstantsService.PROMOTION}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public async Task<IActionResult> Promotion(int id)
        {
            Promotion promotion = await _promotion.GetModelAsync(id);
            if (promotion == null || promotion.NotInUse)
                return GetNotFoundPage();
            string desc = $"<p class=\"mb-2\">{CultureProvider.GetLocalName(promotion.DescRu, promotion.DescEn, promotion.DescTm)}</p>";
            if (promotion.Type is Tp.ProductFree or Tp.Set or Tp.SpecialSetDiscount)
            {
                Product product = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { (int)promotion.SubjectId } } }).FirstOrDefaultAsync();
                if (product.NotInUse)
                {
                    promotion.NotInUse = true;
                    await _promotion.SaveChangesAsync();
                    return GetNotFoundPage();
                }
                string additional = $"<h5><a href=\"/{PathService.GetModelUrl(ConstantsService.PRODUCT, product.Id)}\"><img class=\"img-fluid mx-2\" style=\"max-height: 50px\" src=\"{PathService.GetImageRelativePath(ConstantsService.PRODUCT, product.Id)}\" />{IProduct.GetProductNameCounted(product)}</a> - ";
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
                    return GetNotFoundPage();
                }
            }
            ViewBag.Desc = desc;
            if (promotion == null)
                return GetNotFoundPage();
            CreateMetaData(ConstantsService.PROMOTION, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.PROMOTIONS), CultureProvider.GetLocalName(promotion.NameRu, promotion.NameEn, promotion.NameTm), true);
            return View();
        }
        [Route(ConstantsService.SEARCH)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public IActionResult Search(string search)
        {
            if (string.IsNullOrEmpty(search))
                return RedirectToAction("");
            CreateMetaData(ConstantsService.SEARCH, _breadcrumbs.GetBreadcrumbs(), _localizer["search"].Value, true);
            return View();
        }
        //[Route($"{{special}}")]
        [Route(ConstantsService.NOVELTIES)]
        [Route(ConstantsService.RECOMMENDED)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400)]
        public IActionResult Special()
        {
            CreateMetaData(CultureProvider.Path.Remove(0, 1), _breadcrumbs.GetBreadcrumbs(), null, true);
            return View();
        }
        [Route(ConstantsService.LIKED)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
        public IActionResult Liked(string special)
        {
            CreateMetaData(special, _breadcrumbs.GetBreadcrumbs(), null, true);
            return View();
        }
        [Route($"{ConstantsService.PRODUCTS}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
        public async Task<JsonResult> Products(string model, string id, bool productsOnly, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp = 100, string search = null)
        {
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
                    IList<int> categoryIds = _category.GetAllCategoryIdsHaveProductsByParentId(categoryId);
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
            int? sw = GetScreenWidth();
            IEnumerable<string> products = uiProducts.Select(p => IProduct.GetHtmlProduct(p, sw, 12, 6, 6, 4, 4, 3, 3, 3));
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
                lastPage,
                noProduct = uiProducts.Any() ? null : _localizer["noProduct"].Value
            });
        }
        [Route($"{ConstantsService.PRODUCT}/{{id}}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10800)]
        public async Task<IActionResult> Product(int id)
        {
            Product product = await _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { id } } }).Include(p => p.Model).ThenInclude(m => m.Category).FirstOrDefaultAsync();
            if (product == null)
                return GetNotFoundPage();
            if (product.NotInUse || product.Model.Category.NotInUse)
            {
                product = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { id } } }).FirstOrDefaultAsync();
                string localName = IProduct.GetProductNameCounted(product);
                CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
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
                            List = new Collection<AdminSpecsValueMod>()
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
            CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), namedProducts.FirstOrDefault(x => x.Item2.Id == id).Item1, false, false);
            ViewBag.Specs = nameUsedSpecs;
            ViewBag.SpecIds = namedSpecIds;
            ViewBag.Current = product.Id;
            return View(namedProducts);
        }
        [Route(ConstantsService.COMPARISON)]
        public async Task<IActionResult> Comparison()
        {
            bool isScaled = HttpContext.Request.Cookies.TryGetValue("scaled", out string scaled);
            CreateMetaData(ConstantsService.COMPARISON, _breadcrumbs.GetBreadcrumbs());
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
        public IActionResult Articles()
        {
            IEnumerable<Heading> headings = _heading.GetModels(new Dictionary<string, object> { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } });
            CreateMetaData(ConstantsService.ARTICLES, _breadcrumbs.GetBreadcrumbs());
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
                articles += $"<div class=\"col-{col} col-sm-{sm} col-lg-{lg} col-xxl-{xxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\">{IImage.GetImageHtml(PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id), 0, 300, "auto", "120px", a.Name, "rounded-1")}</div><p>{a.Name}</p><div class=\"text-end\"><small>{a.Date.ToShortDateString()}</small></div></a></div>";
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 259200)]
        public async Task<IActionResult> Article(int id)
        {
            Article article = await _article.GetModelAsync(id);
            if (article == null)
                return GetNotFoundPage();
            CreateMetaData(ConstantsService.ARTICLE, _breadcrumbs.GetModelBreadcrumbs(ConstantsService.ARTICLES), article.Name);
            ViewBag.ScreenWidth = GetScreenWidth() - 20;
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
                    CreateMetaData(ConstantsService.ABOUT, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.ABOUT].Value, true);
                    content = await System.IO.File.ReadAllTextAsync(_path.GetHtmlAboutBodyPath(CultureProvider.Lang));
                    break;
                case ConstantsService.DELIVERY:
                    CreateMetaData(ConstantsService.DELIVERY, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.DELIVERY].Value, true);
                    content = await System.IO.File.ReadAllTextAsync(_path.GetHtmlDeliveryBodyPath(CultureProvider.Lang));
                    break;
            }
            return View(null, content);
        }
        [Route($"{ConstantsService.CART}")]
        public async Task<IActionResult> Cart()
        {
            CreateMetaData(ConstantsService.CART, _breadcrumbs.GetBreadcrumbs(), _localizer[ConstantsService.CART].Value);
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
        public async Task<JsonResult> Order([FromForm] string orders, [FromForm] DeliveryData deliveryData)
        {
            CartOrder[] cartOrders = JsonService.Deserialize<CartOrder[]>(orders);
            if (cartOrders == null || !cartOrders.Any())
                return new JsonResult(new
                {
                    noOrders = true
                });
            await _invoice.CreateInvoice(_user.GetCurrentUser().Result?.Id, cartOrders, deliveryData);
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
        public IActionResult NotFoundPage()
        {
            CreateMetaData();
            return View();
        }
        [HttpGet("recovery/newpin/{guid}")]
        public async Task<IActionResult> PinRecovery(Guid guid)
        {
            LoginResponse loginResponse = await _login.NewPINAsync(guid);
            if (loginResponse.Result == LoginResponse.R.Error)
                return GetNotFoundPage();
            CreateMetaData();
            return View(loginResponse);
        }
        private IActionResult GetNotFoundPage()
        {
            CreateMetaData();
            return View("NotFoundPage");
        }
        private async Task<IActionResult> AccountGeneric(User user)
        {
            CreateMetaData(ConstantsService.ACCOUNT, _breadcrumbs.GetBreadcrumbs(), user.Name);
            ViewBag.Invoices = await _invoice.GetUserInvoicesAsync(user.Id);
            return View(user);
        }
        private void CreateMetaData(string pageName = null, IList<Breadcrumb> breadcrumbs = null, string modelName = null, bool filterScript = false, bool isPageName = true)
        {
            IQueryable<Category> mainCategories = _category.GetActiveCategoriesByParentId(0);
            ViewBag.MainCategories = mainCategories;
            ViewData[ConstantsService.TITLE] = modelName != null ? $" - {modelName}".ToLower() : pageName != null ? $" - {_localizer[pageName]}".ToLower() : null;
            ViewData[ConstantsService.DESCRIPTION] = string.Format(_localizer[$"desc-{(pageName == null ? "home" : "model")}"], CultureProvider.SiteName, modelName);
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
        //        CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
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
        //            CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName, false, false);
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
        //        CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), localName2, false, false);
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
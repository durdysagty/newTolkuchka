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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
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
        private readonly static int _deliveryFree = 500;
        private readonly static int _deliveryPrice = 20;

        public HomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache)
        {
            _localizer = localizer;
            _breadcrumbs = breadcrumbs;
            _path = path;
            _category = category;
            _brand = brand;
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
        public IActionResult Index()
        {
            ViewBag.MainSlides = _slide.GetSlidesByLayoutAsync(Layout.Main).ToList();
            IQueryable<Category> mainCategories = _category.GetActiveCategoriesByParentId(0);
            ViewBag.MainCategories = mainCategories;
            CreateMetaData();
            return View();
        }
        [Route($"{ConstantsService.INDEX}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
        public async Task<string> Items()
        {
            int count = 6;
            int? width = GetScreenWidth();
            if (width is < 1200 and > 768)
                count = 4;
            else if (width < 351)
                count = 4;
            const int col = 12;
            const int xs = 6;
            const int sm = 4;
            const int md = 3;
            const int lg = 3;
            const int xl = 2;
            const int xxl = 2;
            const int xxxl = 2;
            static IEnumerable<string> GetHtmlProducts(IList<IEnumerable<UIProduct>> products)
            {
                return products.Select(u => IProduct.GetHtmlProduct(u, col, xs, sm, md, lg, xl, xxl, xxxl));
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
            IList<IEnumerable<UIProduct>> newUIProducts = await _product.GetFullModels().OrderByDescending(p => p.Id).Where(p => p.IsNew && !p.NotInUse && !p.Model.Category.NotInUse).Take(count).Select(p => _product.GetUIProduct(new Product[1] { p })).ToListAsync();
            IEnumerable<string> newProducts = GetHtmlProducts(newUIProducts);
            IList<IEnumerable<UIProduct>> recUIProducts = await _product.GetFullModels().OrderByDescending(p => p.Id).Where(p => p.IsRecommended && !p.NotInUse && !p.Model.Category.NotInUse).Take(count).Select(p => _product.GetUIProduct(new Product[1] { p })).ToListAsync();
            IEnumerable<string> recProducts = GetHtmlProducts(recUIProducts);
            IQueryable<Category> mainCategories = _category.GetActiveCategoriesByParentId(0);
            IList<(Category, IEnumerable<string>)> categories = new List<(Category, IEnumerable<string>)>();
            foreach (Category category in mainCategories)
            {
                IList<int> categoryIds = _category.GetAllCategoryIdsHaveProductsByParentId(category.Id);
                if (categoryIds.Any())
                {
                    IList<IEnumerable<UIProduct>> products = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, categoryIds } }).OrderByDescending(p => p.Id).Where(p => !p.NotInUse && !p.Model.Category.NotInUse).Take(count).Select(p => _product.GetUIProduct(new Product[1] { p })).ToListAsync();
                    (Category, IEnumerable<string>) mainCategory = (category, GetHtmlProducts(products));
                    categories.Add(mainCategory);
                }
            }
            string template = "<p class=\"fs-5 px-3 mb-2 border-bottom border-primary\"><a href=\"/{0}\">{1}</a></p><div class=\"row\">{2}</div>";
            string html = string.Format(template, ConstantsService.NOVELTIES, _localizer[ConstantsService.NOVELTIES].Value, GetItems(newProducts));
            html += string.Format(template, ConstantsService.RECOMMENDED, _localizer[ConstantsService.RECOMMENDED].Value, GetItems(recProducts));
            foreach ((Category, IEnumerable<string>) c in categories)
            {
                html += string.Format(template, PathService.GetModelUrl(ConstantsService.CATEGORY, c.Item1.Id), CultureProvider.GetLocalName(c.Item1.NameRu, c.Item1.NameEn, c.Item1.NameTm), GetItems(c.Item2));
            }
            IEnumerable<Article> articles = _article.GetModels(new Dictionary<string, object> { { ConstantsService.CULTURE, CultureProvider.CurrentCulture } }).OrderByDescending(a => a.Id).Take(count);
            if (articles.Count() >= count)
            {
                string articleStrings = null;
                foreach (Article a in articles)
                {
                    articleStrings += $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\"><img class=\"rounded-1\" style=\"width: auto; height: 70px\" src=\"{PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id)}\" alt=\"{a.Name}\" /></div><div><small class=\"small text-center\">{a.Name}</small></div></a></div>";
                }
                html += $"<div class=\"bg-light\"><div class=\"row justify-content-center\">{articleStrings}</div><div class=\"d-flex justify-content-end\"><a href=\"{ConstantsService.ARTICLES}\"><small>{_localizer["all-articles"].Value}</small></a></div></div>";
            }
            return html;
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
            IQueryable<Brand> brands = _brand.GetModels();
            CreateMetaData(ConstantsService.BRANDS, _breadcrumbs.GetBreadcrumbs());
            return View(brands);
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
            CreateMetaData(ConstantsService.BRAND, _breadcrumbs.GetBrandBreadcrumbs(), brand.Name, true);
            return View();
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
            IList<(string, Product)> namedProducts = new List<(string, Product)>();
            // to select Specs from any of product for change products by specs value
            IList<ModelWithList<SpecsValue>> nameUsedSpecs = products.FirstOrDefault().Model.ModelSpecs.Where(s => s.IsNameUse).OrderBy(s => s.Spec.NamingOrder).Select(ms => new ModelWithList<SpecsValue>
            {
                Id = ms.Spec.Id,
                Name = CultureProvider.GetLocalName(ms.Spec.NameRu, ms.Spec.NameEn, ms.Spec.NameTm),
                Is = ms.Spec.IsImaged,
                List = new Collection<SpecsValue>()
            }).ToList();
            IEnumerable<int> namedSpecIds = nameUsedSpecs.Select(ms => ms.Id);
            foreach (Product p in products)
            {
                IEnumerable<SpecsValue> psvs = p.ProductSpecsValues.Select(psv => psv.SpecsValue).Where(x => namedSpecIds.Contains(x.SpecId));
                string name = IProduct.GetProductNameSingle(p, psvs);
                namedProducts.Add((name, p));
                foreach (SpecsValue sv in psvs)
                {
                    ModelWithList<SpecsValue> spec = nameUsedSpecs.FirstOrDefault(s => s.Id == sv.SpecId);
                    if (!spec.List.Any(s => s.Id == sv.Id))
                        spec.List.Add(sv);
                }
            }
            CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.Model.CategoryId), namedProducts.FirstOrDefault(x => x.Item2.Id == id).Item1, false, false);
            ViewBag.Specs = nameUsedSpecs;
            ViewBag.SpecIds = namedSpecIds;
            ViewBag.Current = product.Id;
            return View(namedProducts);
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
        public IActionResult Special(string special)
        {
            CreateMetaData(special, _breadcrumbs.GetBreadcrumbs(), null, true);
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
        public async Task<JsonResult> Products(string model, string id, bool productsOnly/*, int[] t*/, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp = 100, string search = null)
        {
            IList<Product> list = new List<Product>();
            IQueryable<Product> targetProducts = null;
            bool brandsOnly = false;
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
                    // get all products in by brand
                    targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.BRAND, new int[] { int.Parse(id) } } });
                    break;
                case ConstantsService.SEARCH:
                    brandsOnly = true;
                    var words = search.Trim().Split(" ");
                    targetProducts = _product.GetFullModels().Where(p => p.Model.Type.NameRu.Contains(words[0]) || p.Model.Type.NameEn.Contains(words[0]) || p.Model.Type.NameTm.Contains(words[0]) || p.Model.Brand.Name.Contains(words[0]) || p.Model.Line.Name.Contains(words[0]) || p.Model.Name.Contains(words[0]) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[0]) || psv.SpecsValue.NameEn.Contains(words[0]) || psv.SpecsValue.NameTm.Contains(words[0])) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[0]) || psvm.SpecsValueMod.NameEn.Contains(words[0]) || psvm.SpecsValueMod.NameTm.Contains(words[0])));
                    if (list.Any())
                    {
                        for (var i = 1; i < words.Length; i++)
                        {
                            targetProducts = targetProducts.Where(p => p.Model.Type.NameRu.Contains(words[i]) || p.Model.Type.NameEn.Contains(words[i]) || p.Model.Type.NameTm.Contains(words[i]) || p.Model.Brand.Name.Contains(words[i]) || p.Model.Line.Name.Contains(words[i]) || p.Model.Name.Contains(words[i]) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[i]) || psv.SpecsValue.NameEn.Contains(words[i]) || psv.SpecsValue.NameTm.Contains(words[i])) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[i]) || psvm.SpecsValueMod.NameEn.Contains(words[i]) || psvm.SpecsValueMod.NameTm.Contains(words[i])));
                            if (!list.Any())
                                break;
                        }
                    }
                    break;
                case ConstantsService.NOVELTIES:
                    targetProducts = _product.GetFullModels().Where(p => p.IsNew);
                    break;
                case ConstantsService.RECOMMENDED:
                    targetProducts = _product.GetFullModels().Where(p => p.IsRecommended);
                    break;
                case ConstantsService.LIKED:
                    targetProducts = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, JsonSerializer.Deserialize<IList<int>>(id) } });
                    break;
            }
            if (targetProducts.Any())
            {
                list = await targetProducts.Where(p => !p.NotInUse && !p.Model.Category.NotInUse).ToListAsync();
            }
            if (!list.Any())
                return new JsonResult(new
                {
                    products = list,
                    noProduct = _localizer["noProductAbsolutly"].Value
                });
            IList<IEnumerable<UIProduct>> /*IEnumerable<UIProduct>*/ uiProducts = _product.GetUIData(productsOnly, brandsOnly, list,/* t, */b, v, minp, maxp, sort, page, pp,/* out IList<AdminType> types, */out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
            IEnumerable<string> products = uiProducts.Select(p => IProduct.GetHtmlProduct(p, 12, 6, 6, 4, 4, 3, 3, 3));
            return new JsonResult(new
            {
                products,
                brands = new
                {
                    name = _localizer[ConstantsService.BRAND].Value,
                    brands
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
                articles += $"<div class=\"col-{col} col-sm-{sm} col-lg-{lg} col-xxl-{xxl}\"><a href=\"/{ConstantsService.ARTICLE}/{a.Id}\"><div class=\"p-1 text-center vrw\"><img class=\"rounded-1\" style=\"width: auto; height: 120px\" src=\"{PathService.GetImageRelativePath(ConstantsService.ARTICLE, a.Id)}\" alt=\"{a.Name}\" /></div><p>{a.Name}</p><div class=\"text-end\"><small>{a.Date.ToShortDateString()}</small></div></a></div>";
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
            CreateMetaData(ConstantsService.ARTICLE, _breadcrumbs.GetArticleBreadcrumbs(), article.Name);
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
        public JsonResult CartData([FromBody] CartOrder[] orders)
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
                deliveryFree = _deliveryFree,
                deliveryPrice = _deliveryPrice
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
            Invoice invoice = new()
            {
                Date = DateTimeOffset.Now.ToUniversalTime(),
                Buyer = deliveryData.Name,
                InvoiceAddress = deliveryData.Address,
                InvoiceEmail = deliveryData.Email,
                InvoicePhone = deliveryData.Phone,
                CurrencyRate = CurrencyService.Currency.RealRate,
                CurrencyId = CurrencyService.Currency.Id,
                UserId = _user.GetCurrentUser().Result?.Id,
                Language = CultureProvider.Lang == ConstantsService.TK ? ConstantsService.TM : CultureProvider.Lang,
            };
            await _invoice.AddModelAsync(invoice);
            decimal sum = 0;
            foreach (CartOrder cartOrder in cartOrders)
            {
                Product product = await _product.GetModelAsync(cartOrder.Id);
                if (product != null)
                    for (int i = 0; i < cartOrder.Quantity; i++)
                    {
                        Order order = new()
                        {
                            ProductId = product.Id,
                            InvoiceId = invoice.Id,
                            OrderPrice = IProduct.GetConvertedPrice(product.NewPrice != null ? (decimal)product.NewPrice : product.Price)
                        };
                        await _order.AddModelAsync(order, false);
                        if (sum < _deliveryFree)
                            sum += cartOrder.Price;
                    }
            }
            if (sum < _deliveryFree)
                invoice.DeliveryCost = _deliveryPrice;
            await _order.SaveChangesAsync();
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
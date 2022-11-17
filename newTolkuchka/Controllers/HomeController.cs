using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

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
        private readonly IUser _user;
        private readonly IInvoice _invoice;
        private readonly IOrder _order;
        private readonly static int _deliveryFree = 500;
        private readonly static int _deliveryPrice = 20;

        public HomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IProduct product, ISlide slide, IUser user, IOrder order, IInvoice invoice)
        {
            _localizer = localizer;
            _breadcrumbs = breadcrumbs;
            _path = path;
            _category = category;
            _brand = brand;
            _product = product;
            _slide = slide;
            _user = user;
            _order = order;
            _invoice = invoice;
        }
        #endregion
        [Route(ConstantsService.SLASH)]
        [Route(ConstantsService.HOME)]
        public IActionResult Index()
        {
            ViewBag.MainSlides = _slide.GetSlidesByLayoutAsync(Layout.Main).ToList();
            IQueryable<Product> newProducts = _product.GetFullProducts().OrderByDescending(p => p.Id).Where(p => p.IsNew && !p.NotInUse).Take(4);
            IEnumerable<UIProduct> newUIProducts = newProducts.Select(p => IProduct.GetUIProduct(p, null));
            ViewBag.NewProducts = newUIProducts.Select(u => IProduct.GetHtmlProduct(u, _localizer["add"], 3, 6, 6));
            IQueryable<Product> recProducts = _product.GetFullProducts().OrderByDescending(p => p.Id).Where(p => p.IsRecommended && !p.NotInUse).Take(4);
            IEnumerable<UIProduct> recUIProducts = recProducts.Select(p => IProduct.GetUIProduct(p, null));
            ViewBag.RecProducts = recUIProducts.Select(r => IProduct.GetHtmlProduct(r, _localizer["add"], 3, 6, 6));
            ViewBag.MainCategories = _category.GetCategoriesByParentId(0);
            CreateMetaData();
            return View();
        }

        [Route(ConstantsService.CATEGORIES)]
        public async Task<IActionResult> Categories()
        {
            IEnumerable<CategoryTree> categories = await _category.GetCategoryTree();
            CreateMetaData(ConstantsService.CATEGORIES, _breadcrumbs.GetBreadcrumbs());
            return View(categories);
        }

        [Route(ConstantsService.BRANDS)]
        public IActionResult Brands()
        {
            IQueryable<Brand> brands = _brand.GetModels();
            CreateMetaData(ConstantsService.BRANDS, _breadcrumbs.GetBreadcrumbs());
            return View(brands);
        }

        [Route($"{ConstantsService.CATEGORY}/{{id}}")]
        public async Task<IActionResult> Category(int id)
        {
            Category category = await _category.GetModelAsync(id);
            string localName = CultureProvider.GetLocalName(category.NameRu, category.NameEn, category.NameTm);
            ViewBag.Categories = _category.GetCategoriesByParentId(id);
            #region old version
            // maybe possibility to optimize
            //IList<int> categoryIds = _category.GetAllCategoryIdsHaveProductsByParentId(id);
            //if (!categoryIds.Any())
            //    return View();
            //IList<Product> products = new List<Product>();
            //foreach (int cId in categoryIds)
            //{
            //    IQueryable<Product> p = _product.GetFullProducts(cId);
            //    if (p.Any())
            //        products = products.Concat(p).ToList();
            //}
            //if (!products.Any())
            //    return View();
            //IList<UIProduct> uiProducts = _product.GetUIData(products, out IList<Brand> brands, out IList<Filter> filters);
            //ViewBag.Brands = brands;
            //ViewBag.Filters = filters;
            #endregion
            CreateMetaData(ConstantsService.CATEGORY, await _breadcrumbs.GetCategoryBreadcrumbsAsync(category.ParentId), localName, true);
            return View();
        }

        [Route($"{ConstantsService.BRAND}/{{id}}")]
        public async Task<IActionResult> Brand(int id)
        {
            Brand brand = await _brand.GetModelAsync(id);
            CreateMetaData(ConstantsService.BRAND, _breadcrumbs.GetBrandBreadcrumbs(), brand.Name, true);
            return View();
        }

        [Route($"{ConstantsService.PRODUCT}/{{id}}")]
        public async Task<IActionResult> Product(int id)
        {
            //Product product = await _product.GetFullProducts(null, null, null, null, new int[1] { id }).AsNoTracking().FirstOrDefaultAsync();
            Product product = await _product.GetFullProducts(null, null, null, null, null, new int[1] { id }).FirstOrDefaultAsync();
            string localName = IProduct.GetProductName(product);
            CreateMetaData(ConstantsService.PRODUCT, await _breadcrumbs.GetProductBreadcrumbs(product.CategoryId), localName, true, false);
            IQueryable<Product> products = _product.GetFullProducts(null, null, null, product.ModelId).Where(p => !p.NotInUse).AsNoTrackingWithIdentityResolution();
            if (!products.Any())
                return View();
            // to select Specs from any of product for change products by specs value
            IList<ModelWithList<SpecsValue>> specs = product.Model.ModelSpecs.Where(s => s.Spec.NamingOrder != null).OrderBy(s => s.Spec.NamingOrder).Select(ms => new ModelWithList<SpecsValue>
            {
                Id = ms.Spec.Id,
                Name = CultureProvider.GetLocalName(ms.Spec.NameRu, ms.Spec.NameEn, ms.Spec.NameTm),
                Is = ms.Spec.IsImaged,
                List = new Collection<SpecsValue>()
            }).ToList();
            foreach (Product p in products)
            {
                foreach (SpecsValue sv in p.ProductSpecsValues.Select(psv => psv.SpecsValue).Where(x => x.Spec.NamingOrder != null))
                {
                    ModelWithList<SpecsValue> spec = specs.FirstOrDefault(s => s.Id == sv.SpecId);
                    if (!spec.List.Any(s => s.Id == sv.Id))
                        spec.List.Add(sv);
                }
            }
            ViewBag.Specs = specs;
            ViewBag.Current = product.Id;
            return View(products);
        }

        [Route(ConstantsService.SEARCH)]
        public IActionResult Search(string search)
        {
            if (string.IsNullOrEmpty(search))
                return RedirectToAction("");
            CreateMetaData("search", _breadcrumbs.GetBreadcrumbs(), _localizer["search"].Value, true);
            return View();
        }

        [Route(ConstantsService.ABOUT)]
        [Route($"{ConstantsService.DELIVERY}")]
        public async Task<IActionResult> Content()
        {
            string p = CultureProvider.Path.TrimStart(new char[] { '/' });
            string content = null;
            switch (p) {
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

        [Route($"{ConstantsService.PRODUCTS}")]
        public async Task<JsonResult> Products(string model, int id, bool productsOnly, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp = 20, string search = null)
        {
            IList<Product> list = new List<Product>();
            switch (model)
            {
                case ConstantsService.CATEGORY:
                    // get all products in the category including subcategories
                    // maybe possibility to optimize
                    IList<int> categoryIds = _category.GetAllCategoryIdsHaveProductsByParentId(id);
                    if (!categoryIds.Any())
                        return new JsonResult(new
                        {
                            products = list,
                            noProduct = _localizer["noProductAbsolutly"].Value
                        });
                    list = await _product.GetFullProducts(categoryIds).Where(p => !p.NotInUse).ToListAsync();
                    break;
                case ConstantsService.BRAND:
                    // get all products in by brand
                    list = await _product.GetFullProducts(null, new[] { id }).Where(p => !p.NotInUse).ToListAsync();
                    break;
                case ConstantsService.SEARCH:
                    var words = search.Trim().Split(" ");
                    list = await _product.GetFullProducts().Where(p => p.Type.NameRu.Contains(words[0]) || p.Type.NameEn.Contains(words[0]) || p.Type.NameTm.Contains(words[0]) || p.Brand.Name.Contains(words[0]) || p.Line.Name.Contains(words[0]) || p.Model.Name.Contains(words[0]) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[0]) || psv.SpecsValue.NameEn.Contains(words[0]) || psv.SpecsValue.NameTm.Contains(words[0])) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[0]) || psvm.SpecsValueMod.NameEn.Contains(words[0]) || psvm.SpecsValueMod.NameTm.Contains(words[0]))).Where(p => !p.NotInUse).ToListAsync();
                    if (list.Any())
                    {
                        for (var i = 1; i < words.Length; i++)
                        {
                            list = list.Where(p => p.Type.NameRu.Contains(words[i]) || p.Type.NameEn.Contains(words[i]) || p.Type.NameTm.Contains(words[i]) || p.Brand.Name.Contains(words[i]) || p.Line.Name.Contains(words[i]) || p.Model.Name.Contains(words[i]) || p.ProductSpecsValues.Any(psv => psv.SpecsValue.NameRu.Contains(words[i]) || psv.SpecsValue.NameEn.Contains(words[i]) || psv.SpecsValue.NameTm.Contains(words[i])) || p.ProductSpecsValueMods.Any(psvm => psvm.SpecsValueMod.NameRu.Contains(words[i]) || psvm.SpecsValueMod.NameEn.Contains(words[i]) || psvm.SpecsValueMod.NameTm.Contains(words[i]))).Where(p => !p.NotInUse).ToList();
                            if (!list.Any())
                                break;
                        }
                    }
                    break;
            }
            if (!list.Any())
                return new JsonResult(new
                {
                    products = list,
                    noProduct = _localizer["noProductAbsolutly"].Value
                });
            IEnumerable<UIProduct> uiProducts = _product.GetUIData(productsOnly, list, t, b, v, minp, maxp, sort, page, pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
            IEnumerable<string> products = uiProducts.Select(p => IProduct.GetHtmlProduct(p, _localizer["add"]));
            return new JsonResult(new
            {
                products,
                types = new
                {
                    name = _localizer["type"].Value,
                    types
                },
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
            CartOrder[] cartOrders = JsonSerializer.Deserialize<CartOrder[]>(orders, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (cartOrders == null || !cartOrders.Any())
                return new JsonResult(new
                {
                    noOrders = true
                });
            Invoice invoice = new()
            {
                Date = DateTimeOffset.Now,
                Buyer = deliveryData.Name,
                InvoiceAddress = deliveryData.Address,
                InvoiceEmail = deliveryData.Email,
                InvoicePhone = deliveryData.Phone,
                CurrencyRate = CurrencyService.Currency.RealRate,
                CurrencyId = CurrencyService.Currency.Id,
                UserId = _user.GetCurrentUser()?.Result.Id,
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
                    HttpContext.Response.Cookies.Delete(Secrets.userCookie);
                    return RedirectToAction("Index");
                }
            }
            return await AccountGeneric(user);
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
            bool isWidthSet = HttpContext.Request.Cookies.TryGetValue("w", out string width);
            if (isWidthSet)
            {
                int w = int.Parse(width);
                ViewBag.WindowWidth = w;
                if (ViewBag.MainCategories == null && w < ConstantsService.MOBILEWIDTH)
                    ViewBag.MainCategories = _category.GetCategoriesByParentId(0);
            }
            else
            {
                if (ViewBag.MainCategories == null)
                    ViewBag.MainCategories = _category.GetCategoriesByParentId(0);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
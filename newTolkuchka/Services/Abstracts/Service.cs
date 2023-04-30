using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using Type = System.Type;
using ModelsType = newTolkuchka.Models.Type;
using static newTolkuchka.Services.CultureProvider;
using System.Collections.ObjectModel;

namespace newTolkuchka.Services.Abstracts
{
    public abstract class Service<T, TAdmin> : BaseService, IAction<T, TAdmin> where T : class where TAdmin : class
    {
        private protected readonly AppDbContext _con;
        private protected readonly ICacheClean _cacheClean;
        public Service(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean ) : base(localizer)
        {
            _con = con;
            _cacheClean = cacheClean;
        }

        public async Task<T> GetModelAsync(int id)
        {
            T model = (T)await _con.FindAsync(typeof(T), id);
            return model;
        }

        public async Task<T> GetModelAsync(Guid? id)
        {
            T model = (T)await _con.FindAsync(typeof(T), id);
            return model;
        }
        public IQueryable<T> GetModels(Dictionary<string, object> paramsList = null)
        {
            IQueryable<T> models = _con.Set<T>();
            // only if paramList not null T type Name to check
            if (paramsList != null)
            {
                Type modelType = typeof(T);
                switch (modelType.Name.ToLower())
                {
                    case ConstantsService.ARTICLE:
                        IQueryable<Article> articles = models as IQueryable<Article>;
                        if (paramsList.TryGetValue(ConstantsService.CULTURE, out object value))
                        {
                            Culture culture = (Culture)value;
                            articles = articles.Where(x => x.HeadingArticles.Any(h => h.Heading.Language == culture));
                        }
                        if (paramsList.TryGetValue(ConstantsService.HEADING, out value))
                        {
                            int headingId = int.Parse(value.ToString());
                            articles = articles.Where(x => x.HeadingArticles.Any(h => h.HeadingId == headingId));
                        }
                        models = (IQueryable<T>)articles;
                        break;
                    case ConstantsService.HEADING:
                        IQueryable<Heading> headings = models as IQueryable<Heading>;
                        if (paramsList.TryGetValue(ConstantsService.CULTURE, out value))
                        {
                            Culture culture = (Culture)value;
                            headings = headings.Where(x => x.Language == culture);
                        }
                        if (paramsList.TryGetValue(ConstantsService.ARTICLE, out value))
                        {
                            int articleId = (int)value;
                            headings = headings.Where(x => x.HeadingArticles.Any(ha => ha.ArticleId == articleId));
                        }
                        models = (IQueryable<T>)headings;
                        break;
                    case ConstantsService.INVOICE:
                        IQueryable<Invoice> invoices = models as IQueryable<Invoice>;
                        // if we set START date we need to set END date also
                        if (paramsList.TryGetValue(ConstantsService.START, out value))
                        {
                            DateTimeOffset start = DateTimeOffset.Parse(value.ToString());
                            start = start.Offset > TimeSpan.FromSeconds(0) ? start.UtcDateTime + start.Offset : start.UtcDateTime - start.Offset;
                            DateTimeOffset end = DateTimeOffset.Parse(paramsList.GetValueOrDefault(ConstantsService.END).ToString());
                            end = end.Offset > TimeSpan.FromSeconds(0) ? end.UtcDateTime + end.Offset : end.UtcDateTime - end.Offset;
                            invoices = invoices.Where(i => i.IsPaid && i.PaidDate.Value.Date >= start.Date && i.PaidDate.Value.Date <= end.Date);
                        }
                        if (paramsList.TryGetValue(ConstantsService.USER, out value))
                        {
                            int userId = int.Parse(value.ToString());
                            invoices = invoices.Where(x => x.UserId == userId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.CUSTOMERGUID, out value))
                        {
                            Guid customerGiud = Guid.Parse(value.ToString());
                            invoices = invoices.Where(x => x.CustomerGuidId == customerGiud);
                        }
                        models = (IQueryable<T>)invoices;
                        break;
                    case ConstantsService.LINE:
                        IQueryable<Line> lines = models as IQueryable<Line>;
                        if (paramsList.TryGetValue(ConstantsService.BRAND, out value))
                        {
                            int brandId = int.Parse(value.ToString());
                            lines = lines.Where(x => x.BrandId == brandId);
                        }
                        models = (IQueryable<T>)lines;
                        break;
                    case ConstantsService.MODEL:
                        IQueryable<Model> models2 = models as IQueryable<Model>;
                        if (paramsList.TryGetValue(ConstantsService.BRAND, out value))
                        {
                            int brandId = int.Parse(value.ToString());
                            models2 = models2.Where(x => x.BrandId == brandId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.LINE, out value))
                        {
                            int? lineId = value.ToString() == "null" ? null : int.Parse(value.ToString());
                            models2 = models2.Where(x => x.LineId == lineId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.CATEGORY, out value))
                        {
                            int categoryId = int.Parse(value.ToString());
                            models2 = models2.Where(x => x.CategoryId == categoryId);
                        }
                        models = (IQueryable<T>)models2;
                        break;
                    case ConstantsService.PRODUCT:
                        // start filtering products
                        IQueryable<Product> products = models as IQueryable<Product>;
                        if (paramsList.TryGetValue(ConstantsService.CATEGORY, out value))
                        {
                            IList<int> categoryIds = (IList<int>)value;
                            if (categoryIds != null)
                                products = products.Where(p => categoryIds.Any(c => c == p.Model.CategoryId) || p.Model.CategoryModelAdLinks.Any(x => categoryIds.Any(c => c == x.CategoryId)));
                        }
                        if (paramsList.TryGetValue(ConstantsService.BRAND, out value))
                        {
                            IList<int> brandIds = (IList<int>)value;
                            if (brandIds != null)
                                products = products.Where(p => brandIds.Any(b => b == p.Model.BrandId));
                        }
                        if (paramsList.TryGetValue(ConstantsService.TYPE, out value))
                        {
                            int? typeId = int.Parse(value.ToString());
                            if (typeId != null)
                                products = products.Where(p => p.Model.TypeId == typeId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.LINE, out value))
                        {
                            int? lineId = int.Parse(value.ToString());
                            if (lineId != null)
                                products = products.Where(p => p.Model.LineId == lineId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.MODEL, out value))
                        {
                            int? modelId = int.Parse(value.ToString());
                            if (modelId != null)
                                products = products.Where(p => p.ModelId == modelId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.PRODUCT, out value))
                        {
                            IList<int> productIds = (IList<int>)value;
                            if (productIds != null)
                                products = products.Where(p => productIds.Any(x => x == p.Id));
                        }
                        if (paramsList.TryGetValue(ConstantsService.PROMOTION, out value))
                        {
                            int? promotionId = int.Parse(value.ToString());
                            if (promotionId != null)
                                products = products.Where(p => p.PromotionProducts.Any(p => p.PromotionId == promotionId));
                        }
                        models = (IQueryable<T>)products;
                        break;
                    case ConstantsService.SPECSVALUE:
                        IQueryable<SpecsValue> specsValues = models as IQueryable<SpecsValue>;
                        if (paramsList.TryGetValue(ConstantsService.SPEC, out value))
                        {
                            int specId = int.Parse(value.ToString());
                            specsValues = specsValues.Where(x => x.SpecId == specId);
                        }
                        models = (IQueryable<T>)specsValues;
                        break;
                    case ConstantsService.SPECSVALUEMOD:
                        IQueryable<SpecsValueMod> specsValueMods = models as IQueryable<SpecsValueMod>;
                        if (paramsList.TryGetValue(ConstantsService.SPECSVALUE, out value))
                        {
                            int specsValueId = int.Parse(value.ToString());
                            specsValueMods = specsValueMods.Where(x => x.SpecsValueId == specsValueId);
                        }
                        models = (IQueryable<T>)specsValueMods;
                        break;
                    default:
                        break;
                }
                if (modelType.Name.ToLower() == ConstantsService.PRODUCT)
                {

                }
            }
            return models;
        }

        public IQueryable<T> GetFullModels(Dictionary<string, object> paramsList = null)
        {
            IQueryable<T> fullModels = GetModels(paramsList);
            Type modelType = typeof(T);
            switch (modelType.Name.ToLower())
            {
                case ConstantsService.ARTICLE:
                    IQueryable<Article> articles = fullModels as IQueryable<Article>;
                    fullModels = (IQueryable<T>)articles.Include(a => a.HeadingArticles).ThenInclude(a => a.Heading);
                    break;
                case ConstantsService.BRAND:
                    IQueryable<Brand> brands = fullModels as IQueryable<Brand>;
                    fullModels = (IQueryable<T>)brands.Include(b => b.Models);
                    break;
                case ConstantsService.CATEGORY:
                    IQueryable<Category> categories = fullModels as IQueryable<Category>;
                    fullModels = (IQueryable<T>)categories.Include(c => c.Models);
                    break;
                case ConstantsService.EMPLOYEE:
                    IQueryable<Employee> employees = fullModels as IQueryable<Employee>;
                    fullModels = (IQueryable<T>)employees.Include(e => e.Position);
                    break;
                case ConstantsService.INVOICE:
                    if (typeof(TAdmin).Name == "AdminInvoice")
                    {
                        IQueryable<Invoice> invoices = fullModels as IQueryable<Invoice>;
                        fullModels = (IQueryable<T>)invoices.Include(i => i.Currency).Include(i => i.User).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Type).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Brand).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Line).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod).ThenInclude(x => x.SpecsValue);
                        break;
                    }
                    else
                    {
                        IQueryable<Invoice> invoicesForReport = fullModels as IQueryable<Invoice>;
                        fullModels = (IQueryable<T>)invoicesForReport.Include(i => i.Currency).Include(i => i.Orders).ThenInclude(o => o.Purchase).ThenInclude(p => p.PurchaseInvoice).ThenInclude(pi => pi.Currency).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Type).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Brand).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(m => m.Line).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(i => i.Orders).ThenInclude(o => o.Product).ThenInclude(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod);
                        break;
                    }
                case ConstantsService.LINE:
                    IQueryable<Line> lines = fullModels as IQueryable<Line>;
                    fullModels = (IQueryable<T>)lines.Include(l => l.Models).Include(l => l.Brand);
                    break;
                case ConstantsService.MODEL:
                    IQueryable<Model> models = fullModels as IQueryable<Model>;
                    fullModels = (IQueryable<T>)models.Include(m => m.Brand).Include(m => m.Category).Include(m => m.Line).Include(m => m.Type).Include(m => m.Products);
                    break;
                case ConstantsService.POSITION:
                    IQueryable<Position> positions = fullModels as IQueryable<Position>;
                    fullModels = (IQueryable<T>)positions.Include(p => p.Employees);
                    break;
                case ConstantsService.PRODUCT:
                    IQueryable<Product> products = fullModels as IQueryable<Product>;
                    fullModels = (IQueryable<T>)products.Include(p => p.Model).ThenInclude(m => m.Category).Include(p => p.Model).ThenInclude(m => m.Type).Include(p => p.Model).ThenInclude(m => m.Brand).Include(p => p.Model).ThenInclude(m => m.Line).Include(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod).ThenInclude(x => x.SpecsValue).Include(x => x.PromotionProducts.Where(pp => !pp.Promotion.NotInUse)).ThenInclude(x => x.Promotion);
                    break;
                case ConstantsService.PROMOTION:
                    IQueryable<Promotion> promotions = fullModels as IQueryable<Promotion>;
                    fullModels = (IQueryable<T>)promotions.Include(p => p.PromotionProducts);
                    break;
                case ConstantsService.PURCHASEINVOICE:
                    IQueryable<PurchaseInvoice> purchaseInvoices = fullModels as IQueryable<PurchaseInvoice>;
                    fullModels = (IQueryable<T>)purchaseInvoices.Include(p => p.Currency).Include(p => p.Purchases).Include(p => p.Supplier);
                    break;
                case ConstantsService.SPEC:
                    IQueryable<Spec> specs = fullModels as IQueryable<Spec>;
                    fullModels = (IQueryable<T>)specs.Include(s => s.SpecsValues);
                    break;
                case ConstantsService.SPECSVALUE:
                    IQueryable<SpecsValue> specsValues = fullModels as IQueryable<SpecsValue>;
                    fullModels = (IQueryable<T>)specsValues.Include(s => s.ProductSpecsValues);
                    break;
                case ConstantsService.SPECSVALUEMOD:
                    IQueryable<SpecsValueMod> specsValueMods = fullModels as IQueryable<SpecsValueMod>;
                    fullModels = (IQueryable<T>)specsValueMods.Include(s => s.ProductSpecsValueMods);
                    break;
                case ConstantsService.SUPPLIER:
                    IQueryable<Supplier> suppliers = fullModels as IQueryable<Supplier>;
                    fullModels = (IQueryable<T>)suppliers.Include(s => s.PurchaseInvoices);
                    break;
                case ConstantsService.TYPE:
                    IQueryable<ModelsType> types = fullModels as IQueryable<ModelsType>;
                    fullModels = (IQueryable<T>)types.Include(s => s.Models);
                    break;
                case ConstantsService.USER:
                    IQueryable<User> users = fullModels as IQueryable<User>;
                    fullModels = (IQueryable<T>)users.Include(s => s.Invoices);
                    break;
                default:
                    break;
            }
            return fullModels;
        }

        public IEnumerable<TAdmin> GetAdminModels(int page, int pp, out int lastPage, out string pagination, Dictionary<string, object> paramsList = null)
        {
            IList<T> preModels = GetFullModels(paramsList).ToList();
            Type type = typeof(T);
            int toSkip = page * pp;
            IEnumerable<TAdmin> adminModels = null;
            //bool isPaged = false;
            string[] words = null;
            pagination = null;
            lastPage = 0;
            int countBeforeSkip = 0;
            if (paramsList != null && paramsList.TryGetValue(ConstantsService.SEARCH, out object search))
            {
                words = search.ToString().Trim().Split(" ");
            }
            switch (type.Name.ToLower())
            {
                case ConstantsService.ARTICLE:
                    IEnumerable<Article> preArticles = preModels as IEnumerable<Article>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preArticles.Any())
                                preArticles = preArticles.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Date.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.HeadingArticles.Any(ha => ha.Heading.Name.Contains(word, StringComparison.OrdinalIgnoreCase)));
                        }
                    preArticles = preArticles.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preArticles.Select(x => new AdminArticle { Id = x.Id, Name = x.Name, Headings = string.Join(", ", x.HeadingArticles.Select(h => h.Heading.Name)) });
                    break;
                case ConstantsService.BRAND:
                    IEnumerable<Brand> preBrands = preModels as IEnumerable<Brand>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preBrands.Any())
                                preBrands = preBrands.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    adminModels = (IEnumerable<TAdmin>)preBrands.Select(x => new AdminBrand { Id = x.Id, Name = x.Name, IsForHome = x.IsForHome, Models = x.Models.Count }).OrderBy(x => x.Name);
                    break;
                case ConstantsService.CATEGORY:
                    IEnumerable<Category> preCategories = preModels as IEnumerable<Category>;
                    List<AdminCategory> categories = new();
                    const int PADDING = 2;
                    void GetCategoriesByOrder(IEnumerable<Category> parentList, int level)
                    {
                        foreach (var c in parentList)
                        {
                            categories.Add(new AdminCategory
                            {
                                Padding = level * PADDING,
                                Id = c.Id,
                                Name = c.Order + " " + c.NameRu,
                                Models = c.Models.Count
                            });
                            GetCategoriesByOrder(preCategories.Where(x => x.ParentId == c.Id).OrderBy(x => x.Order), level + 1);
                        }
                    }
                    GetCategoriesByOrder(preCategories.Where(x => x.ParentId == 0).OrderBy(x => x.Order), 0);
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (categories.Any())
                                categories = categories.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Models.ToString().Contains(word, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                    adminModels = (IEnumerable<TAdmin>)categories;
                    break;
                case ConstantsService.CURRENCY:
                    IEnumerable<Currency> preCurrencies = preModels as IEnumerable<Currency>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preCurrencies.Any())
                                preCurrencies = preCurrencies.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.CodeName.Contains(word, StringComparison.OrdinalIgnoreCase) || p.PriceRate.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.RealRate.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    adminModels = (IEnumerable<TAdmin>)preCurrencies.Select(x => new AdminCurrency { Id = x.Id, PriceRate = x.PriceRate, RealRate = x.RealRate, CodeName = x.CodeName });
                    break;
                case ConstantsService.EMPLOYEE:
                    IEnumerable<Employee> preEmployees = preModels as IEnumerable<Employee>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preEmployees.Any())
                                preEmployees = preEmployees.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Login.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Position.Name.Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    adminModels = (IEnumerable<TAdmin>)preEmployees.Select(x => new AdminEmployee { Id = x.Id, HumanName = x.Login, Position = x.Position.Name, Level = x.Position.Level });
                    break;
                case ConstantsService.ENTRY:
                    IEnumerable<Entry> preEntries = preModels as IEnumerable<Entry>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preEntries.Any())
                                preEntries = preEntries.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Employee.Contains(word, StringComparison.OrdinalIgnoreCase) || _localizer[p.Act.ToString().ToLower()].Value.Contains(word, StringComparison.OrdinalIgnoreCase) || _localizer[p.Entity.ToString().ToLower()].Value.Contains(word, StringComparison.OrdinalIgnoreCase) || p.EntityId.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.EntityName.Contains(word, StringComparison.OrdinalIgnoreCase) || p.DateTime.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preEntries = preEntries.OrderByDescending(x => x.Id);
                    countBeforeSkip = preEntries.Count();
                    preEntries = preEntries.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preEntries.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preEntries.Select(x => new AdminEntry { Id = x.Id, Employee = x.Employee, Act = _localizer[x.Act.ToString().ToLower()], Entity = _localizer[x.Entity.ToString().ToLower()], EntityId = x.EntityId, EntityName = x.EntityName, Date = x.DateTime });
                    break;
                case ConstantsService.INVOICE:
                    // in the invoice controller & in the report controller we use same Invoice as T model, thus we have to select
                    if (typeof(TAdmin).Name == "AdminInvoice")
                    {
                        IEnumerable<Invoice> preInvoices = preModels as IEnumerable<Invoice>;
                        if (words != null)
                            foreach (string word in words)
                            {
                                if (preInvoices.Any())
                                    preInvoices = preInvoices.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Date.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || (p.User != null && p.User.Email.Contains(word, StringComparison.OrdinalIgnoreCase)) || p.Buyer.Contains(word, StringComparison.OrdinalIgnoreCase) || p.InvoiceAddress.Contains(word, StringComparison.OrdinalIgnoreCase) || p.InvoicePhone.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Currency.CodeName.Contains(word, StringComparison.OrdinalIgnoreCase) || p.CurrencyRate.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                            }
                        preInvoices = preInvoices.OrderByDescending(x => x.Id);
                        countBeforeSkip = preInvoices.Count();
                        preInvoices = preInvoices.Skip(toSkip).Take(pp);
                        pagination = GetPagination(pp, countBeforeSkip, preInvoices.Count(), toSkip, out lastPage);
                        IList<AdminInvoice> adminInvoices = new List<AdminInvoice>();
                        foreach (Invoice x in preInvoices)
                        {
                            AdminInvoice adminInvoice = new()
                            {
                                Id = x.Id,
                                Date = x.Date,
                                User = x.UserId == null ? null : x.User.Email,
                                Buyer = x.Buyer,
                                Address = x.InvoiceAddress,
                                Phone = x.InvoicePhone,
                                Amount = x.Orders.Select(o => o.OrderPrice).Sum() + x.DeliveryCost,
                                CurrencyCodeName = x.Currency.CodeName,
                                CurrencyRate = x.CurrencyRate,
                                IsDelivered = x.IsDelivered,
                                IsPaid = x.IsPaid,
                                Orders = new Collection<AdminOrder>()
                            };
                            foreach (Order o in x.Orders.DistinctBy(o => o.ProductId))
                            {
                                AdminOrder adminOrder = new()
                                {
                                    Id = o.Id,
                                    ProductId = o.ProductId,
                                    Name = IProduct.GetProductNameCounted(o.Product),
                                    OrderPrice = o.OrderPrice,
                                    Quantity = x.Orders.Count(c => c.ProductId == o.ProductId)
                                };
                                adminInvoice.Orders.Add(adminOrder);
                            }
                            adminInvoices.Add(adminInvoice);
                        }
                        adminModels = (IEnumerable<TAdmin>)adminInvoices;
                    }
                    else
                    {
                        IEnumerable<Invoice> preInvoicesForReport = preModels as IEnumerable<Invoice>;
                        List<AdminReportOrder> reportOrders = new();
                        foreach (Invoice invoice in preInvoicesForReport)
                        {
                            foreach (Order order in invoice.Orders)
                            {
                                decimal soldPrice = Math.Round(order.OrderPrice / invoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
                                decimal boughtPrice = Math.Round(order.Purchase.PurchasePrice / order.Purchase.PurchaseInvoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
                                AdminReportOrder reoprtOrder = new()
                                {
                                    Id = order.Id,
                                    InvoiceId = invoice.Id,
                                    InvoiceDate = invoice.Date,
                                    PaidDate = invoice.PaidDate.Value,
                                    ProductId = order.ProductId,
                                    ProductName = IProduct.GetProductNameCounted(order.Product),
                                    OrderPrice = order.OrderPrice,
                                    OrderCurrency = invoice.Currency.CodeName,
                                    OrderCurrencyRate = invoice.CurrencyRate,
                                    SoldPrice = soldPrice,
                                    PurchasePrice = order.Purchase.PurchasePrice,
                                    PurchaseCurrency = order.Purchase.PurchaseInvoice.Currency.CodeName,
                                    PurchaseCurrencyRate = order.Purchase.PurchaseInvoice.CurrencyRate,
                                    BoughtPrice = boughtPrice,
                                    NetProfit = soldPrice - boughtPrice
                                };
                                reportOrders.Add(reoprtOrder);
                            }
                        }
                        adminModels = (IEnumerable<TAdmin>)reportOrders;
                    }
                    break;
                // dependent from a brand, expected no more than 50 lines in the one brand, else skip will confuse
                case ConstantsService.LINE:
                    IEnumerable<Line> preLines = preModels as IEnumerable<Line>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preLines.Any())
                                preLines = preLines.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Brand.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Models.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preLines = preLines.OrderBy(x => x.Name);
                    countBeforeSkip = preLines.Count();
                    preLines = preLines.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preLines.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preLines.Select(x => new AdminLine { Id = x.Id, Name = x.Name, Brand = x.Brand.Name, Models = x.Models.Count });
                    break;
                case ConstantsService.MODEL:
                    IEnumerable<Model> preModels2 = preModels as IEnumerable<Model>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preModels2.Any())
                                preModels2 = preModels2.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Category.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Type.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Brand.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || (p.Line != null && p.Line.Name.Contains(word, StringComparison.OrdinalIgnoreCase)) || p.Products.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preModels2 = preModels2.OrderByDescending(x => x.Id);
                    countBeforeSkip = preModels2.Count();
                    preModels2 = preModels2.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preModels2.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preModels2.Select(x => new AdminModel { Id = x.Id, Name = x.Name, Category = x.Category.NameRu, Type = x.Type.NameRu, Brand = x.Brand.Name, Line = x.Line?.Name, Products = x.Products.Count });
                    break;
                case ConstantsService.POSITION:
                    IEnumerable<Position> prePositions = preModels as IEnumerable<Position>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (prePositions.Any())
                                prePositions = prePositions.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Level.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Employees.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    adminModels = (IEnumerable<TAdmin>)prePositions.Select(x => new AdminPosition { Id = x.Id, Name = x.Name, Level = x.Level, Employees = x.Employees.Count });
                    break;
                case ConstantsService.PRODUCT:
                    IEnumerable<Product> preProducts = preModels as IEnumerable<Product>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preProducts.Any())
                                preProducts = preProducts.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Price.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NewPrice.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Model.Type.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Model.Brand.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || (p.Model.Line != null && p.Model.Line.Name.Contains(word, StringComparison.OrdinalIgnoreCase)) || p.Model.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.ProductSpecsValues.Any(sv => sv.SpecsValue.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase)) || p.ProductSpecsValueMods.Any(svm => svm.SpecsValueMod.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase)));
                        }
                    preProducts = preProducts.OrderByDescending(x => x.Id);
                    countBeforeSkip = preProducts.Count();
                    preProducts = preProducts.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preProducts.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preProducts.Select(x => new AdminProduct { Id = x.Id, Name = IProduct.GetProductNameCounted(x, 1), Category = x.Model.Category.NameRu, Price = x.Price, NewPrice = x.NewPrice, NotInUse = !x.NotInUse, IsRecommended = x.IsRecommended, IsNew = x.IsNew });
                    break;
                case ConstantsService.PROMOTION:
                    IEnumerable<Promotion> prePromotions = preModels as IEnumerable<Promotion>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (prePromotions.Any())
                                prePromotions = prePromotions.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.PromotionProducts.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    prePromotions = prePromotions.OrderByDescending(x => x.Id);
                    countBeforeSkip = prePromotions.Count();
                    prePromotions = prePromotions.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, prePromotions.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)prePromotions.Select(x => new AdminPromotion { Id = x.Id, Name = x.NameRu, Type = _localizer[x.Type.ToString()], Products = x.PromotionProducts.Count });
                    break;
                case ConstantsService.PURCHASEINVOICE:
                    IEnumerable<PurchaseInvoice> prePurchaseInvoices = preModels as IEnumerable<PurchaseInvoice>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (prePurchaseInvoices.Any())
                                prePurchaseInvoices = prePurchaseInvoices.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Date.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Supplier.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Currency.CodeName.Contains(word, StringComparison.OrdinalIgnoreCase) || p.CurrencyRate.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Purchases.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    prePurchaseInvoices = prePurchaseInvoices.OrderByDescending(x => x.Id);
                    countBeforeSkip = prePurchaseInvoices.Count();
                    prePurchaseInvoices = prePurchaseInvoices.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, prePurchaseInvoices.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)prePurchaseInvoices.Select(x => new AdminPurchaseInvoice { Id = x.Id, Date = x.Date, SupplierName = x.Supplier.Name, CurrencyCodeName = x.Currency.CodeName, CurrencyRate = x.CurrencyRate, Purchases = x.Purchases.Count });
                    break;
                case ConstantsService.SLIDE:
                    IEnumerable<Slide> preSlides = preModels as IEnumerable<Slide>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preSlides.Any())
                                preSlides = preSlides.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preSlides = preSlides.OrderByDescending(x => x.Id);
                    countBeforeSkip = preSlides.Count();
                    preSlides = preSlides.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preSlides.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preSlides.Select(x => new AdminSlide { Id = x.Id, Name = x.Name, NotInUse = !x.NotInUse });
                    break;
                case ConstantsService.SPEC:
                    IEnumerable<Spec> preSpecs = preModels as IEnumerable<Spec>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preSpecs.Any())
                                preSpecs = preSpecs.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.SpecsValues.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Order.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NamingOrder.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preSpecs = preSpecs.OrderBy(x => x.Order);
                    countBeforeSkip = preSpecs.Count();
                    preSpecs = preSpecs.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preSpecs.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preSpecs.Select(x => new AdminSpec { Id = x.Id, Name = x.NameRu, Order = x.Order, NamingOrder = x.NamingOrder, SpecsValues = x.SpecsValues.Count, IsFilter = x.IsFilter });
                    break;
                case ConstantsService.SPECSVALUE:
                    IEnumerable<SpecsValue> preSpecsValues = preModels as IEnumerable<SpecsValue>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preSpecsValues.Any())
                                preSpecsValues = preSpecsValues.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.ProductSpecsValues.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preSpecsValues = preSpecsValues.OrderBy(x => x.NameRu);
                    countBeforeSkip = preSpecsValues.Count();
                    preSpecsValues = preSpecsValues.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preSpecsValues.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValues.Select(x => new AdminSpecsValue { Id = x.Id, Name = x.NameRu, Products = x.ProductSpecsValues.Count });
                    break;
                case ConstantsService.SPECSVALUEMOD:
                    IEnumerable<SpecsValueMod> preSpecsValueMods = preModels as IEnumerable<SpecsValueMod>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preSpecsValueMods.Any())
                                preSpecsValueMods = preSpecsValueMods.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.ProductSpecsValueMods.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preSpecsValueMods = preSpecsValueMods.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValueMods.Select(x => new AdminSpecsValueMod { Id = x.Id, Name = x.NameRu, Products = x.ProductSpecsValueMods.Count });
                    break;
                case ConstantsService.SUPPLIER:
                    IEnumerable<Supplier> preSuppliers = preModels as IEnumerable<Supplier>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preSuppliers.Any())
                                preSuppliers = preSuppliers.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || p.PhoneMain.Contains(word, StringComparison.OrdinalIgnoreCase) || p.PurchaseInvoices.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preSuppliers = preSuppliers.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preSuppliers.Select(x => new AdminSupplier { Id = x.Id, Name = x.Name, PhoneMain = x.PhoneMain, PurchaseInvoices = x.PurchaseInvoices.Count });
                    break;
                case ConstantsService.TYPE:
                    IEnumerable<ModelsType> preTypes = preModels as IEnumerable<ModelsType>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preTypes.Any())
                                preTypes = preTypes.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase) || p.Models.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preTypes = preTypes.OrderBy(x => x.NameRu);
                    countBeforeSkip = preTypes.Count();
                    preTypes = preTypes.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preTypes.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preTypes.Select(x => new AdminType { Id = x.Id, Name = x.NameRu, Models = x.Models.Count });
                    break;
                case ConstantsService.USER:
                    IEnumerable<User> preUsers = preModels as IEnumerable<User>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preUsers.Any())
                                preUsers = preUsers.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.Email.Contains(word, StringComparison.OrdinalIgnoreCase) || (p.Name != null && p.Name.Contains(word, StringComparison.OrdinalIgnoreCase)) || (p.Phone != null && p.Phone.Contains(word, StringComparison.OrdinalIgnoreCase)) || p.Invoices.Count.ToString().Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preUsers = preUsers.OrderBy(x => x.Email);
                    countBeforeSkip = preUsers.Count();
                    preUsers = preUsers.Skip(toSkip).Take(pp);
                    pagination = GetPagination(pp, countBeforeSkip, preUsers.Count(), toSkip, out lastPage);
                    adminModels = (IEnumerable<TAdmin>)preUsers.Select(x => new AdminUser { Id = x.Id, HumanName = x.Name, Email = x.Email, Phone = x.Phone, Invoices = x.Invoices.Count });
                    break;
                case ConstantsService.WARRANTY:
                    IEnumerable<Warranty> preWarranties = preModels as IEnumerable<Warranty>;
                    if (words != null)
                        foreach (string word in words)
                        {
                            if (preWarranties.Any())
                                preWarranties = preWarranties.Where(p => p.Id.ToString().Contains(word, StringComparison.OrdinalIgnoreCase) || p.NameRu.Contains(word, StringComparison.OrdinalIgnoreCase));
                        }
                    preWarranties = preWarranties.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preWarranties.Select(x => new AdminWarranty { Id = x.Id, Name = x.NameRu });
                    break;
                default:
                    adminModels = (IEnumerable<TAdmin>)preModels;
                    break;
            }
            return adminModels;
        }

        public virtual bool IsExist(T model, IEnumerable<T> list)
        {
            bool exist = false;
            if (!list.Any())
                return exist;
            Type type = typeof(T);
            IEnumerable<PropertyInfo> nameProperties = type.GetProperties().Where(x => x.Name.Contains("Name"));
            foreach (PropertyInfo p in nameProperties)
            {
                string value = p.GetValue(model).ToString().ToLower();
                exist = list.Where(x => p.GetValue(x).ToString().ToLower() == value).Any();
                if (exist)
                    return exist;
            }
            return exist;
        }

        public async Task<bool> IsBinded(int id)
        {
            string typeName = typeof(T).Name.ToLower();
            switch (typeName)
            {
                case ConstantsService.BRAND:
                    if (await _con.Lines.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    break;
                case ConstantsService.CATEGORY:
                    if (await _con.Categories.Where(x => x.ParentId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(x => x.CategoryId == id).AnyAsync())
                        return true;
                    break;
                case ConstantsService.HEADING:
                    if (await _con.HeadingArticles.Where(x => x.HeadingId == id).AnyAsync())
                        return true;
                    break;
                case ConstantsService.LINE:
                    if (await _con.Models.Where(x => x.LineId == id).AnyAsync())
                        return true;
                    break;
                case ConstantsService.MODEL:
                    return await _con.Products.Where(x => x.ModelId == id).AnyAsync();
                case ConstantsService.POSITION:
                    return await _con.Employees.Where(x => x.PositionId == id).AnyAsync();
                case ConstantsService.PRODUCT:
                    if (await _con.Purchases.Where(x => x.ProductId == id).AnyAsync())
                        return true;
                    if (await _con.Orders.Where(x => x.ProductId == id).AnyAsync())
                        return true;
                    if (await _con.Promotions.Where(x => x.SubjectId == id || x.PromotionProducts.Any(pp => pp.ProductId == id)).AnyAsync())
                        return true;
                    break;
                case ConstantsService.SPEC:
                    return await _con.SpecsValues.Where(x => x.SpecId == id).AnyAsync();
                case ConstantsService.SPECSVALUE:
                    return await _con.ProductSpecsValues.Where(x => x.SpecsValueId == id).AnyAsync();
                case ConstantsService.SPECSVALUEMOD:
                    return await _con.ProductSpecsValueMods.Where(x => x.SpecsValueModId == id).AnyAsync();
                case ConstantsService.TYPE:
                    return await _con.Models.Where(x => x.TypeId == id).AnyAsync();
                case ConstantsService.WARRANTY:
                    return await _con.Models.Where(x => x.WarrantyId == id).AnyAsync();
                case ConstantsService.SUPPLIER:
                    return await _con.PurchaseInvoices.Where(x => x.SupplierId == id).AnyAsync();
                case ConstantsService.PURCHASEINVOICE:
                    return await _con.Purchases.Where(x => x.PurchaseInvoiceId == id).AnyAsync();
                case ConstantsService.PURCHASE:
                    return await _con.Orders.Where(x => x.PurchaseId == id).AnyAsync();
                //case ConstantsService.INVOICE:
                //return await _con.Orders.Where(x => x.InvoiceId == id).AnyAsync();
                case ConstantsService.CURRENCY:
                    // to be corrected
                    return true;
                default:
                    return false;
            }
            return false;
        }
        public int GetModelId(Type type, T model)
        {
            return (int)type.GetProperty("Id").GetValue(model);
        }
    }
}

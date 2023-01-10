using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using Type = System.Type;
using ModelsType = newTolkuchka.Models.Type;

namespace newTolkuchka.Services.Abstracts
{
    //public enum LanVersion { Ru, En, Tm }
    public abstract class Service<T, TAdmin> : BaseService, IAction<T, TAdmin> where T : class where TAdmin : class
    {
        private protected readonly AppDbContext _con;
        public Service(AppDbContext con, IStringLocalizer<Shared> localizer) : base(localizer)
        {
            _con = con;
        }

        public async Task<T> GetModelAsync(int id)
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
                    case ConstantsService.LINE:
                        IQueryable<Line> lines = models as IQueryable<Line>;
                        if (paramsList.TryGetValue(ConstantsService.BRAND, out object value))
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
                        models = (IQueryable<T>)products;
                        break;
                    case ConstantsService.INVOICE:
                        DateTimeOffset start = DateTimeOffset.Parse(paramsList.GetValueOrDefault(ConstantsService.START).ToString());
                        DateTimeOffset end = DateTimeOffset.Parse(paramsList.GetValueOrDefault(ConstantsService.END).ToString());
                        start = start.Offset > TimeSpan.FromSeconds(0) ? start.UtcDateTime + start.Offset : start.UtcDateTime - start.Offset;
                        end = end.Offset > TimeSpan.FromSeconds(0) ? end.UtcDateTime + end.Offset : end.UtcDateTime - end.Offset;
                        DateTimeOffset dateTimeOffset2 = start.UtcDateTime + start.Offset;
                        IQueryable<Invoice> invoices = models as IQueryable<Invoice>;
                        invoices = invoices.Where(i => i.IsPaid && i.PaidDate >= start && i.PaidDate <= end);
                        models = (IQueryable<T>)invoices;
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
                        fullModels = (IQueryable<T>)invoices.Include(i => i.Currency).Include(i => i.Orders).Include(i => i.User);
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
                    fullModels = (IQueryable<T>)models.Include(m => m.Brand).Include(l => l.Category).Include(l => l.Line).Include(l => l.Type).Include(l => l.Products);
                    break;
                case ConstantsService.POSITION:
                    IQueryable<Position> positions = fullModels as IQueryable<Position>;
                    fullModels = (IQueryable<T>)positions.Include(p => p.Employees);
                    break;
                case ConstantsService.PRODUCT:
                    IQueryable<Product> products = fullModels as IQueryable<Product>;
                    fullModels = (IQueryable<T>)products.Include(p => p.Model).ThenInclude(m => m.Type).Include(p => p.Model).ThenInclude(m => m.Brand).Include(p => p.Model).ThenInclude(m => m.Line).Include(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod);
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
                default:
                    break;
            }
            return fullModels;
        }

        public IEnumerable<TAdmin> GetAdminModels(int page, int pp, out int lastPage, out string pagination, Dictionary<string, object> paramsList = null)
        {
            IQueryable<T> preModels = GetFullModels(paramsList);
            Type type = typeof(T);
            int toSkip = page * pp;
            IEnumerable<TAdmin> adminModels = null;
            bool isPaged = false;
            switch (type.Name.ToLower())
            {
                case ConstantsService.BRAND:
                    IEnumerable<Brand> preBrands = preModels as IEnumerable<Brand>;
                    preBrands = preBrands.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preBrands.Select(x => new AdminBrand
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Models = x.Models.Count
                    }).OrderBy(x => x.Name);
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
                    adminModels = (IEnumerable<TAdmin>)categories;
                    break;
                case ConstantsService.CURRENCY:
                    IEnumerable<Currency> preCurrencies = preModels as IEnumerable<Currency>;
                    adminModels = (IEnumerable<TAdmin>)preCurrencies.Select(x => new AdminCurrency
                    {
                        Id = x.Id,
                        PriceRate = x.PriceRate,
                        RealRate = x.RealRate,
                        CodeName = x.CodeName
                    });
                    break;
                case ConstantsService.EMPLOYEE:
                    IEnumerable<Employee> preEmployees = preModels as IEnumerable<Employee>;
                    adminModels = (IEnumerable<TAdmin>)preEmployees.Select(x => new AdminEmployee
                    {
                        Id = x.Id,
                        HumanName = x.Login,
                        Position = x.Position.Name,
                        Level = x.Position.Level
                    });
                    break;
                case ConstantsService.ENTRY:
                    IEnumerable<Entry> preEntries = preModels as IEnumerable<Entry>;
                    preEntries = preEntries.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preEntries.Select(x => new AdminEntry
                    {
                        Id = x.Id,
                        Employee = x.Employee,
                        Act = _localizer[x.Act.ToString().ToLower()],
                        Entity = _localizer[x.Entity.ToString().ToLower()],
                        EntityId = x.EntityId,
                        EntityName = x.EntityName,
                        Date = x.DateTime,
                    });
                    isPaged = true;
                    break;
                case ConstantsService.INVOICE:
                    // in the invoice controller & in the report controller we use same Invoice as T model, thus we have to select
                    if (typeof(TAdmin).Name == "AdminInvoice")
                    {
                        IQueryable<Invoice> preInvoices = preModels as IQueryable<Invoice>;
                        preInvoices = preInvoices.OrderByDescending(x => x.Id);
                        adminModels = (IEnumerable<TAdmin>)preInvoices.Select(x => new AdminInvoice
                        {
                            Id = x.Id,
                            Date = x.Date,
                            User = x.UserId == null ? null : x.User.Email,
                            Buyer = x.Buyer,
                            Address = x.InvoiceAddress,
                            Phone = x.InvoicePhone,
                            CurrencyCodeName = x.Currency.CodeName,
                            CurrencyRate = x.CurrencyRate,
                            Orders = x.Orders.Count,
                            IsDelivered = x.IsDelivered,
                            IsPaid = x.IsPaid
                        });
                        isPaged = true;
                        break;
                    }
                    else
                    {
                        IEnumerable<Invoice> preInvoicesForReport = preModels as IEnumerable<Invoice>;
                        List<AdminReoprtOrder> reportOrders = new();
                        foreach (Invoice invoice in preInvoicesForReport)
                        {
                            foreach (Order order in invoice.Orders)
                            {
                                decimal soldPrice = Math.Round(order.OrderPrice / invoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
                                decimal boughtPrice = Math.Round(order.Purchase.PurchasePrice / order.Purchase.PurchaseInvoice.CurrencyRate, 2, MidpointRounding.AwayFromZero);
                                AdminReoprtOrder reoprtOrder = new()
                                {
                                    Id = order.Id,
                                    InvoiceId = invoice.Id,
                                    InvoiceDate = invoice.Date,
                                    PaidDate = invoice.PaidDate.Value,
                                    ProductId = order.ProductId,
                                    ProductName = IProduct.GetProductName(order.Product),
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
                        break;
                    }
                // dependent from a brand, expected no more than 50 lines in the one brand, else skip will confuse
                case ConstantsService.LINE:
                    IEnumerable<Line> preLines = preModels as IEnumerable<Line>;
                    preLines = preLines.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preLines.Select(x => new AdminLine
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Brand = x.Brand.Name,
                        Models = x.Models.Count
                    });
                    isPaged = true;
                    break;
                case ConstantsService.MODEL:
                    IQueryable<Model> preModels2 = preModels as IQueryable<Model>;
                    preModels2 = preModels2.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preModels2.Select(x => new AdminModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Category = x.Category.NameRu,
                        Type = x.Type.NameRu,
                        Brand = x.Brand.Name,
                        Line = x.Line.Name,
                        Products = x.Products.Count
                    });
                    isPaged = true;
                    break;
                case ConstantsService.POSITION:
                    IEnumerable<Position> prePositions = preModels as IEnumerable<Position>;
                    adminModels = (IEnumerable<TAdmin>)prePositions.Select(x => new AdminPosition
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Level = x.Level,
                        Employees = x.Employees.Count,
                    });
                    break;
                case ConstantsService.PRODUCT:
                    IQueryable<Product> preProducts = preModels as IQueryable<Product>;
                    preProducts = preProducts.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preProducts.Select(p => new AdminProduct
                    {
                        Id = p.Id,
                        Name = IProduct.GetProductName(p, 1),
                        Category = p.Model.Category.NameRu,
                        Price = p.Price,
                        NewPrice = p.NewPrice,
                        NotInUse = !p.NotInUse,
                        IsRecommended = p.IsRecommended,
                        IsNew = p.IsNew
                    });
                    isPaged = true;
                    break;
                case ConstantsService.PURCHASEINVOICE:
                    IEnumerable<PurchaseInvoice> prePurchaseInvoices = preModels as IEnumerable<PurchaseInvoice>;
                    prePurchaseInvoices = prePurchaseInvoices.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)prePurchaseInvoices.Select(x => new AdminPurchaseInvoice
                    {
                        Id = x.Id,
                        Date = x.Date,
                        SupplierName = x.Supplier.Name,
                        CurrencyCodeName = x.Currency.CodeName,
                        CurrencyRate = x.CurrencyRate,
                        Purchases = x.Purchases.Count
                    });
                    isPaged = true;
                    break;
                case ConstantsService.SLIDE:
                    IEnumerable<Slide> preSlides = preModels as IEnumerable<Slide>;
                    preSlides = preSlides.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preSlides.Select(x => new AdminSlide
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NotInUse = !x.NotInUse
                    });
                    isPaged = true;
                    break;
                case ConstantsService.SPEC:
                    IEnumerable<Spec> preSpecs = preModels as IEnumerable<Spec>;
                    preSpecs = preSpecs.OrderBy(x => x.Order);
                    adminModels = (IEnumerable<TAdmin>)preSpecs.Select(x => new AdminSpec
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Order = x.Order,
                        NamingOrder = x.NamingOrder,
                        SpecsValues = x.SpecsValues.Count
                    });
                    break;
                case ConstantsService.SPECSVALUE:
                    IEnumerable<SpecsValue> preSpecsValues = preModels as IEnumerable<SpecsValue>;
                    preSpecsValues = preSpecsValues.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValues.Select(x => new AdminSpecsValue
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Products = x.ProductSpecsValues.Count
                    });
                    break;
                case ConstantsService.SPECSVALUEMOD:
                    IEnumerable<SpecsValueMod> preSpecsValueMods = preModels as IEnumerable<SpecsValueMod>;
                    preSpecsValueMods = preSpecsValueMods.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValueMods.Select(x => new AdminSpecsValueMod
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Products = x.ProductSpecsValueMods.Count
                    });
                    break;
                case ConstantsService.SUPPLIER:
                    IEnumerable<Supplier> preSuppliers = preModels as IEnumerable<Supplier>;
                    preSuppliers = preSuppliers.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preSuppliers.Select(x => new AdminSupplier
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PhoneMain = x.PhoneMain,
                        PurchaseInvoices = x.PurchaseInvoices.Count
                    });
                    break;
                case ConstantsService.TYPE:
                    IEnumerable<ModelsType> preTypes = preModels as IEnumerable<ModelsType>;
                    preTypes = preTypes.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preTypes.Select(x => new AdminType
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Models = x.Models.Count
                    });
                    break;
                case ConstantsService.WARRANTY:
                    IEnumerable<Warranty> preWarranties = preModels as IEnumerable<Warranty>;
                    preWarranties = preWarranties.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preWarranties.Select(x => new AdminWarranty
                    {
                        Id = x.Id,
                        Name = x.NameRu
                    });
                    break;
                default:
                    adminModels = (IEnumerable<TAdmin>)preModels;
                    break;
                    //throw new ArgumentOutOfRangeException(type.Name, $"Not expected type name: {type.Name}");
            }
            if (paramsList != null && paramsList.TryGetValue(ConstantsService.SEARCH, out object search))
            {
                string[] words = search.ToString().Trim().Split(" ");
                // IEnumerable<PropertyInfo> propertiesTest = typeof(TAdmin).GetProperties();
                IEnumerable<PropertyInfo> properties = typeof(TAdmin).GetProperties().Where(p => p.PropertyType.Name is "String" or "Int32" or "DateTimeOffset");
                foreach (string word in words)
                {
                    if (adminModels.Any())
                        adminModels = adminModels.Where(m => properties.Any(p => p.GetValue(m) != null && p.GetValue(m).ToString().Contains(word, StringComparison.OrdinalIgnoreCase)));
                }
            }
            if (isPaged)
            {
                int countBeforeSkip = adminModels.Count();
                adminModels = adminModels.Skip(toSkip).Take(pp);
                pagination = GetPagination(pp, countBeforeSkip, adminModels.Count(), toSkip, out int lp);
                lastPage = lp;
            }
            else
            {
                pagination = null;
                lastPage = 0;
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
                string value = p.GetValue(model).ToString();
                exist = list.Where(x => p.GetValue(x).ToString() == value).Any();
                if (exist)
                    return exist;
            }
            return exist;
        }

        public async Task<bool> IsBinded(int id)
        {
            string typeName = typeof(T).Name;
            switch (typeName)
            {
                case "Brand":
                    if (await _con.Lines.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(x => x.BrandId == id).AnyAsync())
                        return true;
                    break;
                case "Category":
                    if (await _con.Categories.Where(x => x.ParentId == id).AnyAsync())
                        return true;
                    if (await _con.Models.Where(x => x.CategoryId == id).AnyAsync())
                        return true;
                    break;
                case "Line":
                    if (await _con.Models.Where(x => x.LineId == id).AnyAsync())
                        return true;
                    break;
                case "Model":
                    return await _con.Products.Where(x => x.ModelId == id).AnyAsync();
                case "Position":
                    return await _con.Employees.Where(x => x.PositionId == id).AnyAsync();
                case "Product":
                    if (await _con.Purchases.Where(x => x.ProductId == id).AnyAsync())
                        return true;
                    if (await _con.Orders.Where(x => x.ProductId == id).AnyAsync())
                        return true;
                    break;
                case "Spec":
                    return await _con.SpecsValues.Where(x => x.SpecId == id).AnyAsync();
                case "SpecsValue":
                    return await _con.ProductSpecsValues.Where(x => x.SpecsValueId == id).AnyAsync();
                case "SpecsValueMod":
                    return await _con.ProductSpecsValueMods.Where(x => x.SpecsValueModId == id).AnyAsync();
                case "Type":
                    return await _con.Models.Where(x => x.TypeId == id).AnyAsync();
                case "Warranty":
                    return await _con.Models.Where(x => x.WarrantyId == id).AnyAsync();
                case "Supplier":
                    return await _con.PurchaseInvoices.Where(x => x.SupplierId == id).AnyAsync();
                case "PurchaseInvoice":
                    return await _con.Purchases.Where(x => x.PurchaseInvoiceId == id).AnyAsync();
                case "Purchase":
                    return await _con.Orders.Where(x => x.PurchaseId == id).AnyAsync();
                case "Invoice":
                    return await _con.Orders.Where(x => x.InvoiceId == id).AnyAsync();
                case "Currency":
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                    case ConstantsService.PRODUCT:
                        // start filtering products
                        IQueryable<Product> products = models as IQueryable<Product>;
                        if (paramsList.TryGetValue(ConstantsService.CATEGORY, out object value))
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
                            int? typeId = (int?)value;
                            if (typeId != null)
                                products = products.Where(p => p.Model.TypeId == typeId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.LINE, out value))
                        {
                            int? lineId = (int?)value;
                            if (lineId != null)
                                products = products.Where(p => p.Model.LineId == lineId);
                        }
                        if (paramsList.TryGetValue(ConstantsService.MODEL, out value))
                        {
                            int? modelId = (int?)value;
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
                    case ConstantsService.SPECSVALUE:
                        IQueryable<SpecsValue> specsValues = models as IQueryable<SpecsValue>;
                        if (paramsList.TryGetValue(ConstantsService.SPEC, out value))
                        {
                            int specId = (int)value;
                            specsValues = specsValues.Where(x => x.SpecId == specId);
                        }
                        models = (IQueryable<T>)specsValues;
                        break;
                    case ConstantsService.SPECSVALUEMOD:
                        IQueryable<SpecsValueMod> specsValueMods = models as IQueryable<SpecsValueMod>;
                        if (paramsList.TryGetValue(ConstantsService.SPECSVALUE, out value))
                        {
                            int specsValueId = (int)value;
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
                    IQueryable<Invoice> invoices = fullModels as IQueryable<Invoice>;
                    fullModels = (IQueryable<T>)invoices.Include(i => i.Currency).Include(i => i.Orders).Include(i => i.User);
                    break;
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
            switch (type.Name.ToLower())
            {
                case ConstantsService.BRAND:
                    IQueryable<Brand> preBrands = preModels as IQueryable<Brand>;
                    preBrands = preBrands.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preBrands.Skip(toSkip).Take(pp).Select(x => new AdminBrand
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Models = x.Models.Count
                    }).OrderBy(x => x.Name);
                    break;
                case ConstantsService.CATEGORY:
                    IQueryable<Category> preCategories = preModels as IQueryable<Category>;
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
                                // change to models
                                Models = c.Models.Count
                            });
                            GetCategoriesByOrder(preCategories.Where(x => x.ParentId == c.Id).OrderBy(x => x.Order), level + 1);
                        }
                    }
                    GetCategoriesByOrder(preCategories.Where(x => x.ParentId == 0).OrderBy(x => x.Order), 0);
                    adminModels = (IEnumerable<TAdmin>)categories;
                    break;
                case ConstantsService.CURRENCY:
                    IQueryable<Currency> preCurrencies = preModels as IQueryable<Currency>;
                    adminModels = (IEnumerable<TAdmin>)preCurrencies.Select(x => new AdminCurrency
                    {
                        Id = x.Id,
                        PriceRate = x.PriceRate,
                        RealRate = x.RealRate,
                        CodeName = x.CodeName
                    });
                    break;
                case ConstantsService.EMPLOYEE:
                    IQueryable<Employee> preEmployees = preModels as IQueryable<Employee>;
                    adminModels = (IEnumerable<TAdmin>)preEmployees.Select(x => new AdminEmployee
                    {
                        Id = x.Id,
                        HumanName = x.Login,
                        Position = x.Position.Name,
                        Level = x.Position.Level
                    });
                    break;
                case ConstantsService.ENTRY:
                    IQueryable<Entry> preEntries = preModels as IQueryable<Entry>;
                    preEntries = preEntries.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preEntries.Skip(toSkip).Take(pp).Select(x => new AdminEntry
                    {
                        Id = x.Id,
                        Employee = x.Employee,
                        Act = _localizer[x.Act.ToString().ToLower()],
                        Entity = _localizer[x.Entity.ToString().ToLower()],
                        EntityId = x.EntityId,
                        EntityName = x.EntityName,
                        Date = x.DateTime,
                    });
                    break;
                case ConstantsService.INVOICE:
                    IQueryable<Invoice> preInvoices = preModels as IQueryable<Invoice>;
                    preInvoices = preInvoices.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preInvoices.Skip(toSkip).Take(pp).Select(x => new AdminInvoice
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
                    break;
                case ConstantsService.LINE:
                    IQueryable<Line> preLines = preModels as IQueryable<Line>;
                    preLines = preLines.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preLines.Skip(toSkip).Take(pp).Select(x => new AdminLine
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Brand = x.Brand.Name,
                        Models = x.Models.Count
                    });
                    break;
                case ConstantsService.MODEL:
                    IQueryable<Model> preModels2 = preModels as IQueryable<Model>;
                    preModels2 = preModels2.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preModels2.Skip(toSkip).Take(pp).Select(x => new AdminModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Category = x.Category.NameRu,
                        Type = x.Type.NameRu,
                        Brand = x.Brand.Name,
                        Line = x.Line.Name,
                        Products = x.Products.Count
                    });
                    break;
                case ConstantsService.POSITION:
                    IQueryable<Position> prePositions = preModels as IQueryable<Position>;
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
                    adminModels = (IEnumerable<TAdmin>)preProducts.Skip(toSkip).Take(pp).Select(p => new AdminProduct
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
                    break;
                case ConstantsService.PURCHASEINVOICE:
                    IQueryable<PurchaseInvoice> prePurchaseInvoices = preModels as IQueryable<PurchaseInvoice>;
                    prePurchaseInvoices = prePurchaseInvoices.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)prePurchaseInvoices.Skip(toSkip).Take(pp).Select(x => new AdminPurchaseInvoice
                    {
                        Id = x.Id,
                        Date = x.Date,
                        SupplierName = x.Supplier.Name,
                        CurrencyCodeName = x.Currency.CodeName,
                        CurrencyRate = x.CurrencyRate,
                        Purchases = x.Purchases.Count
                    });
                    break;
                case ConstantsService.SLIDE:
                    IQueryable<Slide> preSlides = preModels as IQueryable<Slide>;
                    preSlides = preSlides.OrderByDescending(x => x.Id);
                    adminModels = (IEnumerable<TAdmin>)preSlides.Skip(toSkip).Take(pp).Select(x => new AdminSlide
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NotInUse = !x.NotInUse
                    });
                    break;
                case ConstantsService.SPEC:
                    IQueryable<Spec> preSpecs = preModels as IQueryable<Spec>;
                    preSpecs = preSpecs.OrderBy(x => x.Order);
                    adminModels = (IEnumerable<TAdmin>)preSpecs.Skip(toSkip).Take(pp).Select(x => new AdminSpec
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Order = x.Order,
                        NamingOrder = x.NamingOrder,
                        SpecsValues = x.SpecsValues.Count
                    });
                    break;
                case ConstantsService.SPECSVALUE:
                    IQueryable<SpecsValue> preSpecsValues = preModels as IQueryable<SpecsValue>;
                    preSpecsValues = preSpecsValues.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValues.Skip(toSkip).Take(pp).Select(x => new AdminSpecsValue
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Products = x.ProductSpecsValues.Count
                    });
                    break;
                case ConstantsService.SPECSVALUEMOD:
                    IQueryable<SpecsValueMod> preSpecsValueMods = preModels as IQueryable<SpecsValueMod>;
                    preSpecsValueMods = preSpecsValueMods.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preSpecsValueMods.Skip(toSkip).Take(pp).Select(x => new AdminSpecsValueMod
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Products = x.ProductSpecsValueMods.Count
                    });
                    break;
                case ConstantsService.SUPPLIER:
                    IQueryable<Supplier> preSuppliers = preModels as IQueryable<Supplier>;
                    preSuppliers = preSuppliers.OrderBy(x => x.Name);
                    adminModels = (IEnumerable<TAdmin>)preSuppliers.Skip(toSkip).Take(pp).Select(x => new AdminSupplier
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PhoneMain = x.PhoneMain,
                        PurchaseInvoices = x.PurchaseInvoices.Count
                    });
                    break;
                case ConstantsService.TYPE:
                    IQueryable<ModelsType> preTypes = preModels as IQueryable<ModelsType>;
                    preTypes = preTypes.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preTypes.Skip(toSkip).Take(pp).Select(x => new AdminType
                    {
                        Id = x.Id,
                        Name = x.NameRu,
                        Models = x.Models.Count
                    });
                    break;
                case ConstantsService.WARRANTY:
                    IQueryable<Warranty> preWarranties = preModels as IQueryable<Warranty>;
                    preWarranties = preWarranties.OrderBy(x => x.NameRu);
                    adminModels = (IEnumerable<TAdmin>)preWarranties.Skip(toSkip).Take(pp).Select(x => new AdminWarranty
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
            if (type.Name.ToLower() is ConstantsService.CATEGORY or ConstantsService.CURRENCY or ConstantsService.EMPLOYEE or ConstantsService.POSITION)
            {
                pagination = null;
                lastPage = 0;
            }
            else
            {
                pagination = GetPagination(pp, preModels.Count(), adminModels.Count(), toSkip, out int lp);
                lastPage = lp;
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

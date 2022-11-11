using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ProductService : ServiceFormFile<Product>, IProduct
    {
        //private const int IMAGESMAX = 5;
        private readonly IStringLocalizer<Shared> _localizer;
        public ProductService(AppDbContext con, IPath path, IImage image, IStringLocalizer<Shared> localizer) : base(con, path, image, ConstantsService.PRODUCTMAXIMAGE)
        {
            _localizer = localizer;
        }

        public IQueryable<Product> GetProducts(IList<int> categoryIds = null, IList<int> brandIds = null, int? typeId = null, int? modelId = null, IList<int> productIds = null)
        {
            IQueryable<Product> products = GetModels();
            if (categoryIds != null)
                products = products.Where(p => categoryIds.Any(c => c == p.CategoryId) || p.CategoryProductAdLinks.Any(x => categoryIds.Any(c => c == x.CategoryId)));
            if (brandIds != null)
                products = products.Where(p => brandIds.Any(b => b == p.BrandId));
            if (typeId != null)
                products = products.Where(p => p.TypeId == typeId);
            if (modelId != null)
                products = products.Where(p => p.ModelId == modelId);
            if (productIds != null)
                products = products.Where(p => productIds.Any(x => x == p.Id));
            return products;
        }

        public IQueryable<Product> GetFullProducts(IList<int> categoryIds = null, IList<int> brandIds = null, int? typeId = null, int? modelId = null, IList<int> productIds = null)
        {
            return GetProducts(categoryIds, brandIds, typeId, modelId, productIds).Include(p => p.Type).Include(p => p.Brand).Include(p => p.Line).Include(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod);
        }

        public async Task<Product> GetFullProductAsync(int id)
        {
            return await GetModels().Include(p => p.Type).Include(p => p.Brand).Include(p => p.Line).Include(p => p.Model).ThenInclude(x => x.ModelSpecs).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ThenInclude(x => x.Spec).Include(x => x.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod).Include(x => x.Warranty).FirstOrDefaultAsync(p => p.Id == id);
        }

        public IQueryable<AdminProduct> GetAdminProducts(IList<int> categoryIds, IList<int> brandIds, int page, int pp, out int lastPage, out string pagination)
        {
            IQueryable<Product> preProducts = GetFullProducts(categoryIds, brandIds).OrderByDescending(x => x.Id);
            int toSkip = page * pp;
            IQueryable<AdminProduct> adminProducts = preProducts.Skip(toSkip).Take(pp).Select(p => new AdminProduct
            {
                Id = p.Id,
                Name = IProduct.GetProductName(p, 1),
                Category = p.Category.NameRu,
                Price = p.Price,
                NewPrice = p.NewPrice,
                IsInUse = !p.NotInUse
            });
            pagination = GetPagination(pp, preProducts.Count(), adminProducts.Count(), toSkip, out int lp);
            lastPage = lp;
            return adminProducts;
        }

        public IList<UIProduct> GetUIData(bool productsOnly, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage)
        {
            IList<Product> preProducts = new List<Product>();
            IList<UIProduct> uiProducts = new List<UIProduct>();
            types = productsOnly ? null : new List<AdminType>();
            #region for test
            //if (!productsOnly)
            //    types.Add(new AdminType
            //    {
            //        Id = 3,
            //        Name = "Какашка"
            //    });
            #endregion
            brands = productsOnly ? null : new List<Brand>();
            filters = productsOnly ? null : new List<Filter>();
            min = 0;
            max = 0;
            // prepare selected filters (v) to filter products
            IList<Filter> filterList = null;
            if (v.Any())
            {
                filterList = new List<Filter>();
                foreach (string vItem in v)
                {
                    string[] ids = vItem.Split(',');
                    int filId = int.Parse(ids[0]);
                    int valId = int.Parse(ids[1]);
                    Filter filter = filterList.FirstOrDefault(f => f.Id == filId);
                    filter ??= new()
                    {
                        Id = filId,
                        FilterValues = new List<FilterValue>()
                    };
                    filter.FilterValues.Add(new FilterValue
                    {
                        Id = valId,
                    });
                    filterList.Add(filter);
                }
            }
            foreach (Product p in products)
            {
                if (!productsOnly)
                {
                    // prepare types for filter
                    if (!types.Any(t => t.Id == p.TypeId))
                    {
                        AdminType type = new()
                        {
                            Id = p.TypeId,
                            Name = CultureProvider.GetLocalName(p.Type.NameRu, p.Type.NameEn, p.Type.NameTm)
                        };
                        types.Add(type);
                    }
                    types = types.OrderBy(x => x.Name).ToList();
                    // prepare brands for filter
                    if (!brands.Any(b => b.Id == p.BrandId))
                    {
                        Brand brand = new()
                        {
                            Id = p.BrandId,
                            Name = p.Brand.Name
                        };
                        brands.Add(brand);
                    }
                    brands = brands.OrderBy(x => x.Name).ToList();
                    // prepare filters
                    foreach (ProductSpecsValue psv in p.ProductSpecsValues)
                    {
                        if (psv.SpecsValue.Spec.IsFilter)
                        {
                            Filter filter = filters.FirstOrDefault(f => f.Id == psv.SpecsValue.Spec.Id);
                            if (filter == null)
                            {
                                filter = new Filter()
                                {
                                    Id = psv.SpecsValue.Spec.Id,
                                    Name = CultureProvider.GetLocalName(psv.SpecsValue.Spec.NameRu, psv.SpecsValue.Spec.NameEn, psv.SpecsValue.Spec.NameTm),
                                    Order = psv.SpecsValue.Spec.Order,
                                    IsImaged = psv.SpecsValue.Spec.IsImaged,
                                    FilterValues = new List<FilterValue>()
                                };
                                filters.Add(filter);
                            }
                            if (!filter.FilterValues.Any(fv => fv.Id == psv.SpecsValueId))
                                filter.FilterValues.Add(new FilterValue()
                                {
                                    Id = psv.SpecsValueId,
                                    Name = CultureProvider.GetLocalName(psv.SpecsValue.NameRu, psv.SpecsValue.NameEn, psv.SpecsValue.NameTm),
                                    Image = filter.IsImaged ? PathService.GetImageRelativePath(ConstantsService.SPECSVALUE, psv.SpecsValueId) : null
                                });
                        }
                    }
                    // ordering filters & filter values
                    foreach (var f in filters)
                        f.FilterValues = f.FilterValues.OrderBy(fv => fv.Name).ToList();
                    filters = filters.OrderBy(f => f.Order).ToList();
                    if (min != 0)
                    {
                        min = (int)(p.NewPrice != null ? min > p.NewPrice ? p.NewPrice : min : min > p.Price ? p.Price : min);
                        max = (int)(p.NewPrice != null ? max < p.NewPrice ? p.NewPrice : max : max < p.Price ? p.Price : max);
                    }
                    else
                    {
                        min = (int)(p.NewPrice != null ? p.NewPrice : p.Price);
                        max = (int)(p.NewPrice != null ? p.NewPrice : p.Price);
                    }

                }
                if (t.Any())
                {
                    if (!t.Any(x => p.TypeId == x))
                        continue;
                }
                if (b.Any())
                {
                    if (!b.Any(x => p.BrandId == x))
                        continue;
                }
                if (v.Any())
                {
                    if (!filterList.All(l => l.FilterValues.Any(fv => p.ProductSpecsValues.Any(psv => psv.SpecsValueId == fv.Id))))
                        continue;
                }
                if (minp != 0)
                {
                    decimal price = p.NewPrice != null ? IProduct.GetConvertedPrice((decimal)p.NewPrice) : IProduct.GetConvertedPrice(p.Price);
                    if (price < minp || price > maxp + 1)
                        continue;
                }
                preProducts.Add(p);
            }
            preProducts = preProducts.OrderBy(p => p.NewPrice != null ? p.NewPrice : p.Price).DistinctBy(p => p.ModelId).ToList();
            int toSkip = page * pp;
            int q = preProducts.Count;
            if (sort != Sort.NameAZ && sort != Sort.NameZA)
            {
                preProducts = sort switch
                {
                    Sort.PriceUp => preProducts.Skip(toSkip).Take(pp).ToList(),
                    Sort.PriceDown => preProducts.OrderByDescending(p => p.NewPrice != null ? p.NewPrice : p.Price).Skip(toSkip).Take(pp).ToList(),
                    _ => preProducts.OrderByDescending(p => p.Id).Skip(toSkip).Take(pp).ToList(),
                };
                uiProducts = preProducts.Select(pp => IProduct.GetUIProduct(pp, GetProducts(null, null, null, pp.ModelId).Count())).ToArray();
            }
            else
            {
                uiProducts = preProducts.Select(pp => IProduct.GetUIProduct(pp, GetProducts(null, null, null, pp.ModelId).Count())).ToArray();
                uiProducts = sort switch
                {
                    Sort.NameZA => uiProducts.OrderByDescending(p => p.Name).Skip(toSkip).Take(pp).ToList(),
                    _ => uiProducts.OrderBy(p => p.Name).Skip(toSkip).Take(pp).ToList()
                };
            }
            int q2 = uiProducts.Count;
            //pagination = $"{toSkip + 1} - {(q2 < pp ? toSkip + q2 : toSkip + pp)} {_localizer["of"]} {q}";
            pagination = GetPagination(pp, q, q2, toSkip, out int lp);
            lastPage = lp;
            min = (int)IProduct.GetConvertedPrice(min);
            max = (int)IProduct.GetConvertedPrice(max);
            return uiProducts;
        }

        private string GetPagination(int pp, int total, int pageCount, int toSkip, out int lastPage)
        {
            lastPage = total % pp == 0 ? (total / pp) - 1 : (total / pp);
            string pagination = $"{toSkip + 1} - {(pageCount < pp ? toSkip + pageCount : toSkip + pp)} {_localizer["of"]} {total}";
            return pagination;
        }

        public async Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, int productId = 0)
        {
            IList<Product> products = await GetProducts(null, null, null, modelId).Where(p => p.Id != productId).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ToListAsync();
            if (!products.Any())
                return false;
            if (!specsValues.Any())
            {
                if (products.Where(p => !p.ProductSpecsValues.Any()).Any())
                    return true;
            }
            int[] psvCheck = specsValues.OrderBy(x => x).ToArray();
            foreach (Product p in products)
            {
                int[] psv = p.ProductSpecsValues.Select(x => x.SpecsValueId).OrderBy(x => x).ToArray();
                bool result = psvCheck.SequenceEqual(psv);
                if (result)
                    return true;
            }
            return false;
        }

        public async Task<string[]> GetSpecValuesAsync(int id)
        {
            return await GetProductSpecValues(id).Select(x => x.SpecsValueId.ToString()).ToArrayAsync();
        }

        public async Task<object[]> GetSpecValueModsAsync(int id)
        {
            return await GetProductSpecValueMods(id).Select(x => new
            {
                id = x.SpecsValueModId.ToString(),
                parentId = x.SpecsValueMod.SpecsValueId
            }).ToArrayAsync();
        }

        public async Task AddProductSpecValuesAsync(int id, IList<int> specsValues)
        {
            IList<ProductSpecsValue> productSpecsValues = await GetProductSpecValues(id).ToListAsync();
            IList<ProductSpecsValue> toRemove = productSpecsValues.Where(x => !specsValues.Contains(x.SpecsValueId)).ToList();
            foreach (var psv in toRemove)
            {
                _con.ProductSpecsValues.Remove(psv);
            }
            IList<int> toAdds = specsValues.Where(x => !productSpecsValues.Select(y => y.SpecsValueId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                ProductSpecsValue productSpecsValue = new()
                {
                    ProductId = id,
                    SpecsValueId = toAdd
                };
                await _con.ProductSpecsValues.AddAsync(productSpecsValue);
            }
        }

        public void RemoveProductSpecValuesModelSpecRemovedAsync(int modelId, int specId)
        {
            // all products of given model
            IQueryable<int> productIds = GetProducts(null, null, null, modelId).Select(x => x.Id);
            foreach (int p in productIds)
            {
                IQueryable<ProductSpecsValue> productSpecsValues = GetProductSpecValues(p).Include(x => x.SpecsValue).Where(x => x.SpecsValue.SpecId == specId);
                foreach (ProductSpecsValue psv in productSpecsValues)
                {
                    IQueryable<ProductSpecsValueMod> productSpecsValueMods = GetProductSpecValueMods(psv.ProductId).Include(x => x.SpecsValueMod).ThenInclude(x => x.SpecsValue).Where(x => x.SpecsValueMod.SpecsValue.Id == psv.SpecsValueId);
                    foreach (ProductSpecsValueMod psvm in productSpecsValueMods)
                        _con.ProductSpecsValueMods.Remove(psvm);
                    _con.ProductSpecsValues.Remove(psv);
                }
            }
        }

        public async Task AddProductSpecValueModsAsync(int id, IList<int> specsValueMods)
        {
            IList<ProductSpecsValueMod> productSpecsValueMods = await GetProductSpecValueMods(id).ToListAsync();
            IList<ProductSpecsValueMod> toRemove = productSpecsValueMods.Where(x => !specsValueMods.Contains(x.SpecsValueModId)).ToList();
            foreach (var psvm in toRemove)
            {
                _con.ProductSpecsValueMods.Remove(psvm);
            }
            IList<int> toAdds = specsValueMods.Where(x => !productSpecsValueMods.Select(y => y.SpecsValueModId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                ProductSpecsValueMod productSpecsValueMod = new()
                {
                    ProductId = id,
                    SpecsValueModId = toAdd
                };
                await _con.ProductSpecsValueMods.AddAsync(productSpecsValueMod);
            }
        }

        private IQueryable<ProductSpecsValue> GetProductSpecValues(int id)
        {
            return _con.ProductSpecsValues.Where(x => x.ProductId == id);
        }

        private IQueryable<ProductSpecsValueMod> GetProductSpecValueMods(int id)
        {
            return _con.ProductSpecsValueMods.Include(x => x.SpecsValueMod).Where(x => x.ProductId == id);
        }
    }
}
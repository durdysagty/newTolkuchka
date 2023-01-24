using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ProductService : ServiceFormFile<Product, AdminProduct>, IProduct
    {
        //private const int IMAGESMAX = 5;
        public ProductService(AppDbContext con, IStringLocalizer<Shared> localizer, IPath path, IImage image) : base(con, localizer, path, image, ConstantsService.PRODUCTMAXIMAGE)
        {
        }

        public EditProduct GetEditProduct(int id)
        {
            EditProduct editProduct = GetModels().Where(p => p.Id == id).Select(p => new EditProduct
            {
                Id = id,
                PartNo = p.PartNo,
                Price = p.Price,
                NewPrice = p.NewPrice,
                NotInUse = p.NotInUse,
                IsRecommended = p.IsRecommended,
                IsNew = p.IsNew,
                OnOrder = p.OnOrder,
                DescRu = p.DescRu,
                DescEn = p.DescEn,
                DescTm = p.DescTm,
                BrandId = p.Model.BrandId,
                LineId = p.Model.LineId,
                ModelId = p.ModelId
            }).FirstOrDefault();
            return editProduct;
        }

        public async Task<Product> GetFullProductAsync(int id)
        {
            return await GetFullModels().Include(p => p.Model).ThenInclude(m => m.Warranty).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> GetFullProductAsNoTrackingWithIdentityResolutionAsync(int id)
        {
            return await GetFullModels().Include(p => p.Model).ThenInclude(m => m.Warranty).AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(p => p.Id == id);
        }
        public IList<IEnumerable<UIProduct>> GetUIData(bool productsOnly, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage)
        {
            IList<Product> preProducts = new List<Product>();
            IList<IEnumerable<UIProduct>> uiProducts = new List<IEnumerable<UIProduct>>();
            types = productsOnly ? null : new List<AdminType>();
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
                    if (!types.Any(t => t.Id == p.Model.TypeId))
                    {
                        AdminType type = new()
                        {
                            Id = p.Model.TypeId,
                            Name = CultureProvider.GetLocalName(p.Model.Type.NameRu, p.Model.Type.NameEn, p.Model.Type.NameTm)
                        };
                        types.Add(type);
                    }
                    types = types.OrderBy(x => x.Name).ToList();
                    // prepare brands for filter
                    if (!brands.Any(b => b.Id == p.Model.BrandId))
                    {
                        Brand brand = new()
                        {
                            Id = p.Model.BrandId,
                            Name = p.Model.Brand.Name
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
                    if (!t.Any(x => p.Model.TypeId == x))
                        continue;
                }
                if (b.Any())
                {
                    if (!b.Any(x => p.Model.BrandId == x))
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
            if (!productsOnly)
            {
                // ordering filters & filter values
                foreach (var f in filters)
                {
                    //Regex regex = new Regex(f.Name);
                    f.FilterValues = f.FilterValues.OrderBy(f => f, new CompareService()).ToList();
                    //f.FilterValues = f.FilterValues.OrderBy(fv => fv.Name).ToList();
                }
                filters = filters.OrderBy(f => f.Order).ToList();
            }
            //preProducts = preProducts.OrderBy(p => p.NewPrice != null ? p.NewPrice : p.Price).DistinctBy(p => p.ModelId).ToList();
            // to get minimal price products first
            preProducts = preProducts.OrderBy(p => p.NewPrice != null ? p.NewPrice : p.Price).ToList();
            IList<Product> preProductsDistinct = preProducts.DistinctBy(p => p.ModelId).ToList();
            int toSkip = page * pp;
            int q = preProductsDistinct.Count;
            if (sort != Sort.NameAZ && sort != Sort.NameZA)
            {
                preProductsDistinct = sort switch
                {
                    Sort.PriceUp => preProductsDistinct.Skip(toSkip).Take(pp).ToList(),
                    Sort.PriceDown => preProductsDistinct.OrderByDescending(p => p.NewPrice != null ? p.NewPrice : p.Price).Skip(toSkip).Take(pp).ToList(),
                    _ => preProductsDistinct.OrderByDescending(p => p.Id).Skip(toSkip).Take(pp).ToList(),
                };
                //uiProducts = preProducts.Select(pp => GetUIProduct(pp, GetProducts(null, null, null, null, pp.ModelId).Count())).ToArray();
                //uiProducts = preProductsDistinct.Select(pp => GetUIProduct(pp, pp.ModelId)).ToArray();
                uiProducts = preProductsDistinct.Select(pp => GetUIProduct(preProducts.Where(p => p.ModelId == pp.ModelId).ToList())).ToArray();
            }
            else
            {
                //uiProducts = preProducts.Select(pp => GetUIProduct(pp, GetProducts(null, null, null, null, pp.ModelId).Count())).ToArray();
                //uiProducts = preProductsDistinct.Select(pp => GetUIProduct(pp, p.ModelId)).ToArray();
                uiProducts = preProductsDistinct.Select(pp => GetUIProduct(preProducts.Where(p => p.ModelId == pp.ModelId).ToList())).ToArray();
                uiProducts = sort switch
                {
                    Sort.NameZA => uiProducts.OrderByDescending(p => p.FirstOrDefault().Name).Skip(toSkip).Take(pp).ToList(),
                    _ => uiProducts.OrderBy(p => p.FirstOrDefault().Name).Skip(toSkip).Take(pp).ToList()
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

        public async Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, int productId = 0)
        {
            IList<Product> products = await GetModels(new Dictionary<string, object>() { { ConstantsService.MODEL, modelId } }).Where(p => p.Id != productId).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).ToListAsync();
            //if no products in model then ok
            if (!products.Any())
                return false;
            //if no specsValues && any products with no specsValues in model then not allow
            if (!specsValues.Any())
            {
                if (products.Where(p => !p.ProductSpecsValues.Any()).Any())
                    return true;
            }
            int[] psvCheck = specsValues.ToArray();
            IEnumerable<int[]> psvs = products.Select(p => p.ProductSpecsValues.Select(x => x.SpecsValueId).ToArray());
            bool result = IsSequencesEqual(psvCheck, psvs);
            return result;
        }

        public bool IsSequencesEqual(int[] psvCheck, IEnumerable<int[]> psvs)
        {
            foreach (int[] psv in psvs)
            {
                bool result = psvCheck.OrderBy(x => x).SequenceEqual(psv.OrderBy(x => x));
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
            IQueryable<int> productIds = GetModels(new Dictionary<string, object>() { { ConstantsService.MODEL, modelId } }).Select(x => x.Id);
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

        //public UIProduct GetUIProduct(Product p, int? modelCount)
        //{
        //    return new UIProduct()
        //    {
        //        Id = p.Id,
        //        Name = modelCount != null ? IProduct.GetProductName(p, modelCount) : IProduct.GetProductName(p),
        //        Price = IProduct.GetConvertedPrice(p.Price),
        //        NewPrice = p.NewPrice == null ? null : IProduct.GetConvertedPrice((decimal)p.NewPrice),
        //        ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id),
        //        Recommended = p.IsRecommended ? _localizer["recod"] : null,
        //        New = p.IsNew ? _localizer["newed"] : null
        //    };
        //}
        //public UIProduct GetUIProduct(Product p, IList<Product> sameModels)
        //{
        //    //IEnumerable<Product> products = GetProducts(null, null, null, null, sameModels);
        //    //an only one imaged specsvalue in a product is expected
        //    // if multiple images specasvalues are expected, then not firstordefault and sequenceeauale coulde be used
        //    IEnumerable<(int, decimal, decimal)> imagedProductsIds = sameModels?.DistinctBy(p => p.ProductSpecsValues.Where(psv => psv.SpecsValue.Spec.IsImaged).Select(psv => psv.SpecsValue.Id).FirstOrDefault()).Select(p => (p.Id, IProduct.GetConvertedPrice(p.Price), p.NewPrice == null ? 0 : IProduct.GetConvertedPrice((decimal)p.NewPrice)));
        //    return new UIProduct()
        //    {
        //        Id = p.Id,
        //        Name = sameModels != null ? IProduct.GetProductName(p, sameModels.Count) : IProduct.GetProductName(p),
        //        Price = IProduct.GetConvertedPrice(p.Price),
        //        NewPrice = p.NewPrice == null ? null : IProduct.GetConvertedPrice((decimal)p.NewPrice),
        //        ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id),
        //        Recommended = p.IsRecommended ? _localizer["recod"] : null,
        //        New = p.IsNew ? _localizer["newed"] : null,
        //        Others = sameModels != null ? imagedProductsIds : null
        //    };
        //}
        public IEnumerable<UIProduct> GetUIProduct(IList<Product> sameModels)
        {
            IEnumerable<Product> distinct = sameModels.DistinctBy(p => p.ProductSpecsValues.Where(psv => psv.SpecsValue.Spec.IsImaged).Select(psv => psv.SpecsValue.Id).FirstOrDefault());
            return distinct.Select(p => new UIProduct()
            {
                Id = p.Id,
                Name = sameModels != null ? IProduct.GetProductNameCounted(p, sameModels.Count) : IProduct.GetProductNameCounted(p),
                Price = IProduct.GetConvertedPrice(p.Price),
                NewPrice = p.NewPrice == null ? null : IProduct.GetConvertedPrice((decimal)p.NewPrice),
                ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id),
                Recommended = p.IsRecommended ? _localizer["recod"] : null,
                New = p.IsNew ? _localizer["newed"] : null
            });
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
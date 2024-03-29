﻿using Microsoft.EntityFrameworkCore;
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
        private readonly ISpecsValue _specsValue;
        private readonly ISpecsValueMod _specsValueMod;
        public ProductService(AppDbContext con, ISpecsValue specsValue, ISpecsValueMod specsValueMod, IStringLocalizer<Shared> localizer, IPath path, ICacheClean cacheClean, IImage image) : base(con, localizer, path, cacheClean, image, ConstantsService.PRODUCTMAXIMAGE)
        {
            _specsValue = specsValue;
            _specsValueMod = specsValueMod;
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

        public async Task<ApiProduct> GetApiProductAsync(int id)
        {
            Product product = await GetFullModels(new Dictionary<string, object> { { ConstantsService.PRODUCT, new int[1] { id } } }).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return null;
            ApiProduct apiProduct = new()
            {
                Id = id,
                Name = IProduct.GetProductNameCounted(product),
                Price = IProduct.GetOrderPrice(product),
                Desc = product.Model.DescRu + product.DescRu
            };
            string specs = $"<table><tbody>";
            foreach (ModelSpec ms in product.Model.ModelSpecs.OrderBy(s => s.Spec.Order))
            {
                specs += $"<tr><td>{ms.Spec.NameRu}</td>";
                SpecsValue sv = product.ProductSpecsValues.FirstOrDefault(x => x.SpecsValue.SpecId == ms.SpecId)?.SpecsValue;
                if (sv != null)
                {
                    SpecsValueMod svm = product.ProductSpecsValueMods.FirstOrDefault(x => x.SpecsValueMod.SpecsValueId == sv.Id)?.SpecsValueMod;
                    if (svm != null)
                        specs += $"<td>{svm.NameRu}</td>";
                    else
                        specs += $"<td>{sv.NameRu}</td>";
                }
                else
                {
                    specs += $"<td> - </td>";
                }
                specs += "</tr>";
            }
            specs += "<tbody></table>";
            apiProduct.Specs = specs;
            return apiProduct;
        }
        public IList<IEnumerable<UIProduct>> GetUIData(bool productsOnly, bool brandsOnly, bool typesNeeded, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage)
        {
            IList<Product> preProducts = new List<Product>();
            IList<IEnumerable<UIProduct>> uiProducts = new List<IEnumerable<UIProduct>>();
            types = productsOnly && !typesNeeded ? null : new List<AdminType>();
            brands = productsOnly ? null : new List<Brand>();
            filters = productsOnly || brandsOnly ? null : new List<Filter>();
            if (typesNeeded && t.Length == 1)
                filters = new List<Filter>();
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
                    if (typesNeeded)
                    {
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
                    }
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
                // we need only filters of selected type products
                if (t.Any())
                {
                    if (!t.Any(x => p.Model.TypeId == x))
                        continue;
                }
                // we do it in seperate scope b.o. brands & types in brands, we have to select filters after each type selected and selected type is single by user 
                if ((!productsOnly && !brandsOnly && !typesNeeded) || (typesNeeded && t.Length == 1))
                {
                    // prepare filters
                    foreach (ProductSpecsValue psv in p.ProductSpecsValues.Where(psv => psv.SpecsValue.Spec.IsFilter))
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
                                Image = filter.IsImaged ? PathService.GetImageRelativePath(ConstantsService.SPECSVALUE, psv.SpecsValueId) : null,
                                ImageVersion = filter.IsImaged ? psv.SpecsValue.Version : 0
                            });
                    }
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
            if ((!productsOnly && !brandsOnly && !typesNeeded) || (typesNeeded && t.Length == 1))
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

        public async Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, IList<int> specsValueMods, int productId = 0)
        {
            IList<Product> products = await GetModels(new Dictionary<string, object>() { { ConstantsService.MODEL, modelId } }).Where(p => p.Id != productId).Include(p => p.ProductSpecsValues).ThenInclude(x => x.SpecsValue).Include(p => p.ProductSpecsValueMods).ThenInclude(x => x.SpecsValueMod).Include(p => p.Model).ThenInclude(p => p.ModelSpecs).ToListAsync();
            //if no products in model then ok
            if (!products.Any())
                return false;
            //if no specsValues && any products with no specsValues in model then not allow
            if (!specsValues.Any())
            {
                if (products.Where(p => !p.ProductSpecsValues.Any()).Any())
                    return true;
            }
            IEnumerable<int> nameUsedSpecs = products.FirstOrDefault().Model.ModelSpecs.Where(s => s.IsNameUse).Select(ms => ms.SpecId);
            int[] psvCheck = await _specsValue.GetModels().Where(sv => specsValues.Contains(sv.Id) && nameUsedSpecs.Contains(sv.SpecId)).Select(sv => sv.Id).ToArrayAsync();//specsValues.ToArray();
            int[] psvmCheck = await _specsValueMod.GetModels().Where(svm => specsValueMods.Contains(svm.Id) && psvCheck.Contains(svm.SpecsValueId)).Select(sv => sv.Id).ToArrayAsync();
            IList<int[]> psvs = products.Select(p => p.ProductSpecsValues.Where(psv => nameUsedSpecs.Contains(psv.SpecsValue.SpecId)).Select(x => x.SpecsValueId).ToArray()).ToList();
            foreach (Product p in products)
            {
                int[] psv = p.ProductSpecsValues.Where(psv => nameUsedSpecs.Contains(psv.SpecsValue.SpecId)).Select(x => x.SpecsValueId).ToArray();
                bool isEqual = IsSequencesEqual(psvCheck, new List<int[]> { psv });
                if (isEqual)
                {
                    int[] psvm = p.ProductSpecsValueMods.Where(psvm => psv.Contains(psvm.SpecsValueMod.SpecsValueId)).Select(x => x.SpecsValueModId).ToArray();
                    isEqual = IsSequencesEqual(psvmCheck, new List<int[]> { psvm });
                }
                if (isEqual)
                    return true;
            }
            return false;
            //bool result = IsSequencesEqual(psvCheck, psvs);
            //// if product specsValues is equal we check SpecsValueMod
            //if (result)
            //{
            //    int[] psvmCheck = await _specsValueMod.GetModels().Where(svm => specsValueMods.Contains(svm.Id) && psvCheck.Contains(svm.SpecsValueId)).Select(sv => sv.Id).ToArrayAsync();
            //    IList<int[]> psvms = new List<int[]>();
            //    for (int i = 0; i < psvs.Count(); i++)
            //    {
            //        int[] psvm = products[i].ProductSpecsValueMods.Where(psvm => psvs[i].Contains(psvm.SpecsValueMod.SpecsValueId)).Select(x => x.SpecsValueModId).ToArray();
            //        //int[] psvm = await _specsValueMod.GetModels().Where(svm => psv.Contains(svm.SpecsValueId)).Select(sv => sv.Id).ToArrayAsync();
            //        if (specsValueMods.Count == 0 && psvm.Length == 0)
            //            continue;
            //        psvms.Add(psvm);
            //    }
            //    result = IsSequencesEqual(psvmCheck, psvms);
            //}
            //return result;
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

        public IEnumerable<UIProduct> GetUIProduct(IList<Product> sameModels, bool notCollect = false)
        {
            // to optimize, disclude two Selection
            if (notCollect)
            {
                return sameModels.Select(p => new UIProduct()
                {
                    Id = p.Id,
                    Name = IProduct.GetProductNameCounted(p),
                    Price = IProduct.GetConvertedPrice(p.Price),
                    NewPrice = p.PromotionProducts.Any(pp => pp.Promotion.Type == Tp.Discount) ? IProduct.GetConvertedPrice((decimal)(p.Price - p.Price * p.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Discount).Promotion.Volume / 100)) : p.NewPrice == null ? null : IProduct.GetConvertedPrice((decimal)p.NewPrice),
                    ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id),
                    Recommended = p.IsRecommended ? _localizer["recod"] : null,
                    New = p.IsNew ? _localizer["newed"] : null,
                    Promotions = p.PromotionProducts.Select(p => new UIPromotion
                    {
                        Id = p.Promotion.Id,
                        Volume = p.Promotion.Volume,
                        Quantity = p.Promotion.Quantity,
                        SubjectId = p.Promotion.SubjectId,
                        Name = CultureProvider.GetLocalName(p.Promotion.NameRu, p.Promotion.NameEn, p.Promotion.NameTm),
                        Desc = CultureProvider.GetLocalName(p.Promotion.DescRu, p.Promotion.DescEn, p.Promotion.DescTm)
                    }).ToList(),
                    Version = p.Version
                });
            }
            IEnumerable<Product> distinct1 = sameModels.Where(p => !p.ProductSpecsValueMods.Where(psv => psv.SpecsValueMod.SpecsValue.Spec.IsImaged).Any()).DistinctBy(p => p.ProductSpecsValues.Where(psv => psv.SpecsValue.Spec.IsImaged).Select(psv => psv.SpecsValue.Id).FirstOrDefault());
            IEnumerable<Product> distinct2 = sameModels.DistinctBy(p => p.ProductSpecsValueMods.Where(psv => psv.SpecsValueMod.SpecsValue.Spec.IsImaged).Select(psv => psv.SpecsValueMod.Id).FirstOrDefault());
            IEnumerable<Product> distinct = distinct1.Concat(distinct2).DistinctBy(p => p.Id);
            return distinct.Select(p => new UIProduct()
            {
                Id = p.Id,
                Name = sameModels != null ? IProduct.GetProductNameCounted(p, sameModels.Count) : IProduct.GetProductNameCounted(p),
                Price = IProduct.GetConvertedPrice(p.Price),
                NewPrice = p.PromotionProducts.Any(pp => pp.Promotion.Type == Tp.Discount) ? IProduct.GetConvertedPrice((decimal)(p.Price - p.Price * p.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Discount).Promotion.Volume / 100)) : p.NewPrice == null ? null : IProduct.GetConvertedPrice((decimal)p.NewPrice),
                ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id),
                Recommended = p.IsRecommended ? _localizer["recod"] : null,
                New = p.IsNew ? _localizer["newed"] : null,
                Promotions = p.PromotionProducts.Select(p => new UIPromotion {
                    Id = p.Promotion.Id,
                    Volume = p.Promotion.Volume,
                    Quantity = p.Promotion.Quantity,
                    SubjectId = p.Promotion.SubjectId,
                    Name = CultureProvider.GetLocalName(p.Promotion.NameRu, p.Promotion.NameEn, p.Promotion.NameTm),
                    Desc = CultureProvider.GetLocalName(p.Promotion.DescRu, p.Promotion.DescEn, p.Promotion.DescTm)
                }).ToList(),
                Version = p.Version
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
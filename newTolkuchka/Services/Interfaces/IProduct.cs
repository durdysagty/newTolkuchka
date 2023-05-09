using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public enum Sort { Id = 0, PriceUp = 1, PriceDown = -1, NameAZ = 2, NameZA = -2 }
    public interface IProduct : IActionFormFile<Product, AdminProduct>
    {
        EditProduct GetEditProduct(int id);
        Task<Product> GetFullProductAsync(int id);
        Task<Product> GetFullProductAsNoTrackingWithIdentityResolutionAsync(int id);
        Task<ApiProduct> GetApiProductAsync(int id);
        IList<IEnumerable<UIProduct>> GetUIData(bool productsOnly, bool brandsOnly, bool typesNeeded, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
        Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, IList<int> specsValueMods, int productId = 0);
        bool IsSequencesEqual(int[] psvCheck, IEnumerable<int[]> psvs);
        Task<string[]> GetSpecValuesAsync(int id);
        Task<object[]> GetSpecValueModsAsync(int id);
        Task AddProductSpecValuesAsync(int id, IList<int> productSpecsValues);
        void RemoveProductSpecValuesModelSpecRemovedAsync(int modelId, int specId);
        Task AddProductSpecValueModsAsync(int id, IList<int> productSpecsValueMods);
        IEnumerable<UIProduct> GetUIProduct(IList<Product> sameModels);

        static decimal GetOrderPrice(Product product)
        {
            Promotion discountPromotion = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Discount)?.Promotion;
            decimal orderPrice = GetConvertedPrice(discountPromotion != null ? (decimal)(product.Price - product.Price * discountPromotion.Volume / 100) : product.NewPrice != null ? (decimal)product.NewPrice : product.Price);
            return orderPrice;
        }

        static decimal GetConvertedPrice(decimal price)
        {
            decimal rated = price * CurrencyService.Currency.PriceRate;
            return rated > 100 ? rated % 100 > 20 ? rated % 10 > 2 ? decimal.Round(rated - (rated % 10) + 9, 0) : decimal.Round(rated - (rated % 10) - 1, 0) : decimal.Round(rated - (rated % 100) - 1, 0) : decimal.Round(rated - (rated - (int)rated) + (decimal)0.9, 2);
        }

        // I think we can optimize all stuff of getting product name
        #region productName
        static string GetProductNameCounted(Product product, int? productsInModel = 1)
        {
            if (productsInModel > 1)
            {
                string name = GetProductBaseName(product);
                name += $" ({productsInModel} {CultureProvider.GetLocalName("м.", "m.", "m.")})";
                return name;
            }
            else
            {
                string name = GetProductNameSingle(product, product.ProductSpecsValues.Where(x => product.Model.ModelSpecs.Where(y => y.IsNameUse).Any(z => z.Spec == x.SpecsValue.Spec)).Select(psv => psv.SpecsValue));
                return name;
            }
        }

        static string GetProductNameSingle(Product product, IEnumerable<SpecsValue> specsValues)
        {
            string name = GetProductBaseName(product);
            foreach (SpecsValue s in specsValues.OrderBy(x => x.Spec.NamingOrder))
            {
                SpecsValueMod svm = product.ProductSpecsValueMods.FirstOrDefault(y => y.SpecsValueMod.SpecsValueId == s.Id)?.SpecsValueMod;
                name += svm != null ? ' ' + CultureProvider.GetLocalName(svm.NameRu, svm.NameEn, svm.NameTm) : ' ' + CultureProvider.GetLocalName(s.NameRu, s.NameEn, s.NameTm);
            }
            return name;
        }

        static string GetProductBaseName(Product product)
        {
            string name = CultureProvider.GetLocalName(product.Model.Type.NameRu, product.Model.Type.NameEn, product.Model.Type.NameTm);
            name += ' ' + product.Model.Brand.Name + ' ' + product.Model.Line?.Name + ' ' + product.Model.Name;
            return name;
        }
        #endregion

        static string GetHtmlProduct(IEnumerable<UIProduct> products, int? sw, int col, int xs, int sm, int md, int lg, int xl, int xxl, int xxxl)
        {
            string htmlProducts = string.Empty;
            string i = string.Empty;
            if (products.Count() > 1)
                foreach (UIProduct p in products)
                {
                    string id = $"prod{p.Id}";
                    string[] ids = products.Where(x => x.Id != p.Id).Select(x => $"prod{x.Id}").ToArray();
                    string strIds = string.Join(", ", ids);
                    int width = (products.Count() > 2 && sw <= ConstantsService.PHONEWIDTH) || (products.Count() > 2 && (sw > ConstantsService.XXXLWIDTH || sw == null)) ? 50 : products.Count() > 4 ? 50 : 90;
                    string image = $"<span role=\"button\" onclick=\"changeImage({id}, [{strIds}])\">{IImage.GetImageHtml(p.ImageMain, p.Version, 200, 200, $"{width}%", "auto", p.Name, "pb-1")}</span>";
                    i += image;
                }
            int q = 0;
            foreach (UIProduct p in products)
            {
                string id = $"prod{p.Id}";
                string promotions = null;
                if (p.Promotions.Any())
                {
                    foreach (Promotion promo in p.Promotions)
                    {
                        promotions += $"<div class=\"badge badge-info me-1 mt-1\"><a href=\"/{ConstantsService.PROMOTION}/{promo.Id}\">{CultureProvider.GetLocalName(promo.NameRu, promo.NameEn, promo.NameTm)}</a></div>";
                    }
                }
                string ph = $"<div id=\"{id}\" class=\"{(q == 0 ? "d-block" : "d-none")}\"><div class=\"row\"><div class=\"col-10 col-xs-9 col-sm-10 col-xxxl-9 px-0 ps-1 product-image p-0\"><div class=\"badges\">{(p.Recommended != null ? $"<div class=\"badge badge-danger me-1 mt-1\"><a href=\"/{ConstantsService.RECOMMENDED}\">{p.Recommended}</a></div>" : null)}{(p.New != null ? $"<div class=\"badge badge-secondary me-1 mt-1\"><a href=\"/{ConstantsService.NOVELTIES}\">{p.New}</a></div>" : null)}{promotions?? promotions}</div><a href=\"/{ConstantsService.PRODUCT}/{p.Id}\"><div>{IImage.GetImageHtml(p.ImageMain, p.Version, 200, 200, "100%", "auto", p.Name, "pb-1")}</div><div class=\"px-0\"><div><p class=\"product-font product-name\">{p.Name}</p></div><div class=\"justify-content-end align-items-end\"><div class=\"text-end\">{(p.NewPrice != null ? $"<s class=\"product-oprice me-2\">{p.Price}</s><span class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</span>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></a></div><div class=\"col-2 col-xs-3 col-sm-2 col-xxxl-3 px-0\"><div class=\"text-center\"><div class=\"buttons-list\"><div><button type=\"button\" aria-label=\"{CultureProvider.GetLocalName("добавить в корзину", "add to cart", "sebete goş")}\" name=\"order{p.Id}\" onclick=\"order({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-cart-plus\"></i></button></div><div><button type=\"button\" aria-label=\"{CultureProvider.GetLocalName("добавить в понравивщиеся", "add to liked", "halanlaryma goş")}\" name=\"like{p.Id}\" onclick=\"like({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-heart\"></i></button></div><div><button type=\"button\" aria-label=\"{CultureProvider.GetLocalName("добавить в сравнения", "add to comparison", "deňeşdirmä goş")}\" name=\"scale{p.Id}\" onclick=\"scale({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-scale-balanced\"></i></button></div>{i}</div></div></div></div></div>";
                htmlProducts += ph;
                q++;
            }
            string models = $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl} mb-4\">{htmlProducts}</div>";
            return models;
        }
    }
}

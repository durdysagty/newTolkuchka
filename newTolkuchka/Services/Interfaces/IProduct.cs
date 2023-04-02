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
        IList<IEnumerable<UIProduct>> GetUIData(bool productsOnly, bool brandsOnly, bool typesNeeded, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
        Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, IList<int> specsValueMods, int productId = 0);
        bool IsSequencesEqual(int[] psvCheck, IEnumerable<int[]> psvs);
        Task<string[]> GetSpecValuesAsync(int id);
        Task<object[]> GetSpecValueModsAsync(int id);
        Task AddProductSpecValuesAsync(int id, IList<int> productSpecsValues);
        void RemoveProductSpecValuesModelSpecRemovedAsync(int modelId, int specId);
        Task AddProductSpecValueModsAsync(int id, IList<int> productSpecsValueMods);
        IEnumerable<UIProduct> GetUIProduct(IList<Product> sameModels);

        static decimal GetConvertedPrice(decimal price)
        {
            return decimal.Round(price * CurrencyService.Currency.PriceRate, 2);
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
                    int width = products.Count() > 2 && sw <= ConstantsService.PHONEWIDTH ? 50: products.Count() > 4 ? 50 : 100;
                    string image = $"<span><img width =\"200\" height=\"200\" style=\"width: {width}%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" class=\"pb-1\" role=\"button\" onclick=\"changeImage({id}, [{strIds}])\" /></span>";
                    i += image;
                }
            int q = 0;
            foreach (UIProduct p in products)
            {
                string id = $"prod{p.Id}";
                string promotions = string.Empty;
                if (p.Promotions.Any())
                {
                    foreach (Promotion promotion in p.Promotions)
                    {
                        promotions += $"<li><a class=\"dropdown-item\" href=\"/{PathService.GetModelUrl(ConstantsService.PROMOTION, promotion.Id)}\">{CultureProvider.GetLocalName(promotion.NameRu, promotion.NameEn, promotion.NameTm)}</a></li>";
                    }
                }
                string ph = $"<div id=\"{id}\" class=\"{(q == 0 ? "d-block" : "d-none")}\"><div class=\"row\"><div class=\"col-10 col-xs-9 col-sm-10 px-0 ps-1\">{(promotions != string.Empty ? $"<div class=\"dropdown\"><button class=\"btn dropdown-toggle\" id=\"dropdown{p.Id}\" data-mdb-toggle=\"dropdown\" aria-expanded=\"false\">{CultureProvider.GetPromotionsLocal()} </button><ul class=\"dropdown-menu\" aria-labelledby=\"dropdown{p.Id}\">{promotions}</ul></div>" : "")}<a href=\"/{ConstantsService.PRODUCT}/{p.Id}\"><div class=\"p-2\"><img width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div><div class=\"badge badge-primary me-1\">{p.Recommended}</div><div class=\"badge badge-secondary me-1\">{p.New}</div></div><div class=\"justify-content-end align-items-end\"><div class=\"text-end\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></a></div><div class=\"col-2 col-xs-3 col-sm-2 px-0\"><div class=\"text-center\"><div class=\"buttons-list\"><div><button name=\"order{p.Id}\" onclick=\"order({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-cart-plus\"></i></button></div><div><button name=\"like{p.Id}\" onclick=\"like({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-heart\"></i></button></div><div><button name=\"scale{p.Id}\" onclick=\"scale({p.Id})\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-scale-balanced\"></i></button></div>{i}</div></div></div></div></div>";
                htmlProducts += ph;
                q++;
            }
            string models = $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl} mb-4\">{htmlProducts}</div>";
            return models;
        }
    }
}

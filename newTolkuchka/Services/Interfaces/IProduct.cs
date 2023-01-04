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
        IList<IEnumerable<UIProduct>> /*IList<UIProduct>*/ GetUIData(bool productsOnly, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
        Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, int productId = 0);
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
        static string GetProductName(Product product, int? productsInModel = 1)
        {
            // have to opimize
            string name = CultureProvider.GetLocalName(product.Model.Type.NameRu, product.Model.Type.NameEn, product.Model.Type.NameTm);
            name += ' ' + product.Model.Brand.Name + ' ' + product.Model.Line?.Name + ' ' + product.Model.Name;
            if (productsInModel > 1)
            {
                name += $" ({productsInModel} {CultureProvider.GetLocalName("м.", "m.", "m.")})";
                return name;
            }
            IEnumerable<ProductSpecsValue> productSpecsValues = product.ProductSpecsValues.Where(x => product.Model.ModelSpecs.Where(y => y.IsNameUse).Any(z => z.Spec == x.SpecsValue.Spec)).OrderBy(x => x.SpecsValue.Spec.NamingOrder);
            foreach (ProductSpecsValue s in productSpecsValues)
            {
                SpecsValueMod svm = product.ProductSpecsValueMods.FirstOrDefault(y => y.SpecsValueMod.SpecsValueId == s.SpecsValueId)?.SpecsValueMod;
                name += svm != null ? ' ' + CultureProvider.GetLocalName(svm.NameRu, svm.NameEn, svm.NameTm) : ' ' + CultureProvider.GetLocalName(s.SpecsValue.NameRu, s.SpecsValue.NameEn, s.SpecsValue.NameTm);
            }
            return name;
        }

        static string GetHtmlProduct(IEnumerable<UIProduct> products, int col, int xs, int sm, int md, int lg, int xl, int xxl, int xxxl)
        {
            string htmlProducts = string.Empty;
            string i = string.Empty;
            if (products.Count() > 1)
                foreach (UIProduct p in products)
                {
                    string id = $"prod{p.Id}";
                    string[] ids = products.Where(x => x.Id != p.Id).Select(x => $"prod{x.Id}").ToArray();
                    string strIds = string.Join(", ", ids);
                    string image = $"<span><img width =\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" class=\"pb-1\" role=\"button\" onclick=\"changeImage({id}, [{strIds}])\" /></span>";
                    i += image;
                }
            int q = 0;
            foreach (UIProduct p in products)
            {
                string id = $"prod{p.Id}";
                string ph = $"<div id=\"{id}\" class=\"{(q == 0 ? "d-block" : "d-none")}\"><div class=\"row\"><div class=\"col-10 col-xs-9 col-sm-10 px-0 ps-1\"><a href=\"/product/{p.Id}\"><div class=\"p-2\"><img width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div><div class=\"badge badge-primary me-1\">{p.Recommended}</div><div class=\"badge badge-secondary me-1\">{p.New}</div></div><div class=\"justify-content-end align-items-end\"><div class=\"text-end\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></a></div><div class=\"col-2 col-xs-3 col-sm-2 px-0\"><div class=\"text-center\"><div class=\"buttons-list\"><div><button name=\"order{p.Id}\" onclick=\"order({p.Id})\" type=\"submit\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-cart-plus\"></i></button></div><div><button name=\"like{p.Id}\" onclick=\"like({p.Id})\" type=\"submit\" class=\"btn btn-primary px-2 mb-1\"><i class=\"fas fa-heart\"></i></button></div>{i}</div></div></div></div></div>";
                htmlProducts += ph;
                q++;
            }
            string models = $"<div class=\"col-{col} col-xs-{xs} col-sm-{sm} col-md-{md} col-lg-{lg} col-xl-{xl} col-xxl-{xxl} col-xxxl-{xxxl} mb-4\">{htmlProducts}</div>";
            return models;
        }
    }
}

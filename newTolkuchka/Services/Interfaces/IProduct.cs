using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public enum Sort { Id = 0, PriceUp = 1, PriceDown = -1, NameAZ = 2, NameZA = -2 }
    public interface IProduct : IActionFormFile<Product>
    {
        IQueryable<Product> GetProducts(IList<int> categoryIds = null, IList<int> brandIds = null, int? typeId = null, int? lineId = null, int? modelId = null, IList<int> productIds = null);
        IQueryable<Product> GetFullProducts(IList<int> categoryIds = null, IList<int> brandIds = null, int? typeId = null, int? lineId = null, int? modelId = null, IList<int> productIds = null);
        Task<Product> GetFullProductAsync(int id);
        Task<Product> GetFullProductAsNoTrackingWithIdentityResolutionAsync(int id);
        IQueryable<AdminProduct> GetAdminProducts(IList<int> categoryIds, IList<int> brandIds, int? lineId, int? modelId, int page, int pp, out int lastPage, out string pagination);
        IList<IEnumerable<UIProduct>> /*IList<UIProduct>*/ GetUIData(bool productsOnly, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
        Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, int productId = 0);
        Task<string[]> GetSpecValuesAsync(int id);
        Task<object[]> GetSpecValueModsAsync(int id);
        Task AddProductSpecValuesAsync(int id, IList<int> productSpecsValues);
        void RemoveProductSpecValuesModelSpecRemovedAsync(int modelId, int specId);
        Task AddProductSpecValueModsAsync(int id, IList<int> productSpecsValueMods);
        UIProduct GetUIProduct(Product p, IList<Product> sameModels);
        IEnumerable<UIProduct> GetUIProduct2(IList<Product> sameModels);

        static decimal GetConvertedPrice(decimal price)
        {
            return decimal.Round(price * CurrencyService.Currency.PriceRate, 2);
        }
        static string GetProductName(Product product, int? productsInModel = 1)
        {
            // have to opimize
            string name = CultureProvider.GetLocalName(product.Type.NameRu, product.Type.NameEn, product.Type.NameTm);
            name += ' ' + product.Brand.Name + ' ' + product.Line?.Name + ' ' + product.Model.Name;
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

        //static string GetHtmlProduct(UIProduct p, string add, int xl = 4, int md = 6, int sm = 12, int col = 12)
        //{
        //    string product = null;
        //    if (p.Others == null)
        //        product = $"<div class=\"col-{col} col-sm-{sm} col-md-{md} col-xl-{xl} mb-4\"><a href=\"/product/{p.Id}\"><div class=\"row\"><div class=\"col-6 px-0\"><img width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"col-6 d-flex flex-column px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div><div class=\"badge badge-primary me-1\">{p.Recommended}</div><div class=\"badge badge-secondary me-1\">{p.New}</div></div><div class=\"flex-grow-1 d-flex justify-content-end align-items-end\"><div class=\"text-end pe-5\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></div></a><div class=\"text-center py-1\"><button name=\"order{p.Id}\" onclick=\"order({p.Id})\" type=\"submit\" class=\"btn btn-primary mx-3\">{add}</button></div></div>";
        //    else
        //    {
        //        string imgId = $"img{p.Id}";
        //        string btnId = $"btn{p.Id}";
        //        string prc = $"prc{p.Id}";
        //        string prcNew = $"prcNew{p.Id}";
        //        IEnumerable<string> images = p.Others?.Select(o => $"<img width =\"200\" height=\"200\" style=\"width: auto; height: 2.5rem\" alt=\"{p.Name}\" src=\"{PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", o.Item1)}\" class=\"pb-1\" role=\"button\" onclick=\"changeImage({imgId}, {btnId}, {o.Item1})\" />");
        //        string i = string.Join("", images);
        //        product = $"<div class=\"col-{col} col-sm-{sm} col-md-{md} col-xl-{xl} mb-4\"><a href=\"/product/{p.Id}\"><div class=\"row\"><div class=\"col-6 px-0\"><img id=\"{imgId}\" width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"col-6 d-flex flex-column px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div><div class=\"badge badge-primary me-1\">{p.Recommended}</div><div class=\"badge badge-secondary me-1\">{p.New}</div></div><div class=\"flex-grow-1 d-flex justify-content-end align-items-end\"><div class=\"text-end pe-5\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></div></a><div class=\"row py-1\"><div class=\"col-8\">{i}</div><div class=\"col-4\"><button id=\"{btnId}\" name=\"order{p.Id}\" onclick=\"orderByImage({imgId})\" type=\"submit\" class=\"btn btn-primary px-2 text-nowrap\">{add}</button></div></div></div>";
        //    }
        //    return product;
        //}

        static string GetHtmlProduct(IEnumerable<UIProduct> products, string add, int xl = 4, int md = 6, int sm = 12, int col = 12)
        {
            string htmlProducts = string.Empty;
            string i = string.Empty;
            if (products.Count() > 1)
                foreach (UIProduct p in products)
                {
                    string id = $"prod{p.Id}";
                    string[] ids = products.Where(x => x.Id != p.Id).Select(x => $"prod{x.Id}").ToArray();
                    string strIds = string.Join(", ", ids);
                    string image = $"<img width =\"200\" height=\"200\" style=\"width: auto; height: 2.5rem\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" class=\"pb-1\" role=\"button\" onclick=\"changeImage({id}, [{strIds}])\" />";
                    i += image;
                }
            int q = 0;
            foreach (UIProduct p in products)
            {
                string id = $"prod{p.Id}";
                string ph = $"<div id=\"{id}\" class=\"{(q == 0 ? "d-block" : "d-none")}\"><a href=\"/product/{p.Id}\"><div class=\"row\"><div class=\"col-6 px-0\"><img width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"col-6 d-flex flex-column px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div><div class=\"badge badge-primary me-1\">{p.Recommended}</div><div class=\"badge badge-secondary me-1\">{p.New}</div></div><div class=\"flex-grow-1 d-flex justify-content-end align-items-end\"><div class=\"text-end pe-5\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></div></a><div class=\"row py-1\"><div class=\"col-8\">{i}</div><div class=\"col-4\"><button name=\"order{p.Id}\" onclick=\"order({p.Id})\" type=\"submit\" class=\"btn btn-primary px-2 text-nowrap\">{add}</button></div></div></div>";
                htmlProducts += ph;
                q++;
            }
            string models = $"<div class=\"col-{col} col-sm-{sm} col-md-{md} col-xl-{xl} mb-4\">{htmlProducts}</div>";
            return models;
        }
    }
}

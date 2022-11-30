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
        IList<UIProduct> GetUIData(bool productsOnly, IList<Product> products, int[] t, int[] b, string[] v, int minp, int maxp, Sort sort, int page, int pp, out IList<AdminType> types, out IList<Brand> brands, out IList<Filter> filters, out int min, out int max, out string pagination, out int lastPage);
        Task<bool> CheckProductSpecValues(int modelId, IList<int> specsValues, int productId = 0);
        Task<string[]> GetSpecValuesAsync(int id);
        Task<object[]> GetSpecValueModsAsync(int id);
        Task AddProductSpecValuesAsync(int id, IList<int> productSpecsValues);
        void RemoveProductSpecValuesModelSpecRemovedAsync(int modelId, int specId);
        Task AddProductSpecValueModsAsync(int id, IList<int> productSpecsValueMods);

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

        static UIProduct GetUIProduct(Product p, int? modelCount)
        {
            return new UIProduct()
            {
                Id = p.Id,
                Name = modelCount != null ? GetProductName(p, modelCount) : GetProductName(p),
                Price = GetConvertedPrice(p.Price),
                NewPrice = p.NewPrice == null ? null : GetConvertedPrice((decimal)p.NewPrice),
                ImageMain = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", p.Id)
            };
        }

        static string GetHtmlProduct(UIProduct p, string add, int xl = 4, int md = 6, int sm = 12, int col = 12)
        {
            string product = $"<div class=\"col-{col} col-sm-{sm} col-md-{md} col-xl-{xl} mb-4\"><a href=\"/product/{p.Id}\"><div class=\"row\"><div class=\"col-6 px-0\"><img width=\"200\" height=\"200\" style=\"width: 100%; height: auto\" alt=\"{p.Name}\" src=\"{p.ImageMain}\" /></div><div class=\"col-6 d-flex flex-column px-0\"><div><p class=\"product-font\">{p.Name}</p></div><div class=\"flex-grow-1 d-flex justify-content-end align-items-end\"><div class=\"text-end pe-5\">{(p.NewPrice != null ? $"<s class=\"product-oprice\">{p.Price}</s><p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.NewPrice}</p>" : $"<p class=\"fs-6 product-price\">{CurrencyService.Currency.CodeName} {p.Price}</p>")}</div></div></div></div></a><div class=\"text-center py-1\"><button name=\"order{p.Id}\" onclick=\"order({p.Id})\" type=\"submit\" class=\"btn btn-primary mx-3\">{add}</button></div></div>";
            return product;
        }
    }
}

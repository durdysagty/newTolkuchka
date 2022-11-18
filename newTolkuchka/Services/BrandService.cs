using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class BrandService : ServiceFormFile<Brand>, IBrand
    {
        //private const int IMAGESMAX = 1;
        public BrandService(AppDbContext con, IPath path, IImage image, IStringLocalizer<Shared> localizer) : base(con, localizer, path, image, ConstantsService.BRANDMAXIMAGE)
        {
        }

        public IEnumerable<AdminBrand> GetAdminBrands()
        {
            IEnumerable<AdminBrand> brands = GetModels().Select(x => new AdminBrand
            {
                Id = x.Id,
                Name = x.Name,
                Products = x.Products.Count
            }).OrderBy(x => x.Name);
            return brands;
        }
    }
}

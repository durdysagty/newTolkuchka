using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class BrandService : ServiceFormFile<Brand, AdminBrand>, IBrand
    {
        //private const int IMAGESMAX = 1;
        public BrandService(AppDbContext con, IPath path, IImage image, IStringLocalizer<Shared> localizer) : base(con, localizer, path, image, ConstantsService.BRANDMAXIMAGE)
        {
        }
    }
}

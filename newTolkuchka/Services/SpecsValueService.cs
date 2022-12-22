using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecsValueService : ServiceFormFile<SpecsValue, AdminSpecsValue>, ISpecsValue
    {
        private const int IMAGESMAX = 1;
        public SpecsValueService(AppDbContext con, IStringLocalizer<Shared> localizer, IPath path, IImage image) : base(con, localizer, path, image, IMAGESMAX)
        {
        }
    }
}
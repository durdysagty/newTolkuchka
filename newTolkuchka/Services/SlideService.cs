using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SlideService : ServiceFormFile<Slide, AdminSlide>, ISlide
    {
        //private const int IMAGESMAX = 3;
        public SlideService(AppDbContext con, IStringLocalizer<Shared> localizer, IPath path, IImage image) : base(con, localizer, path, image, ConstantsService.SLIDEMAXIMAGE)
        {
        }

        public IQueryable<Slide> GetSlidesByLayoutAsync(Layout layout)
        {
            IQueryable<Slide> slides = GetModels().Where(s => s.Layout == layout && !s.NotInUse);
            return slides;
        }
    }
}

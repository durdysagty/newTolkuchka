using Microsoft.Extensions.Caching.Memory;
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
        public SlideService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, IPath path, ICacheClean cacheClean, IImage image) : base(con, memoryCache, localizer, path, cacheClean, image, ConstantsService.LOCALMAXIMAGE)
        {
        }

        public IQueryable<Slide> GetSlidesByLayoutAsync(Layout layout)
        {
            IQueryable<Slide> slides = GetModels().Where(s => s.Layout == layout && !s.NotInUse);
            return slides;
        }
    }
}

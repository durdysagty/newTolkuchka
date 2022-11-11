using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SlideService : ServiceFormFile<Slide>, ISlide
    {
        //private const int IMAGESMAX = 3;
        public SlideService(AppDbContext con, IPath path, IImage image) : base(con, path, image, ConstantsService.SLIDEMAXIMAGE)
        {
        }

        public IQueryable<Slide> GetSlidesByLayoutAsync(Layout layout)
        {
            IQueryable<Slide> slides = GetModels().Where(s => s.Layout == layout && !s.NotInUse);
            return slides;
        }
    }
}

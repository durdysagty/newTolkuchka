using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISlide : IActionFormFile<Slide>
    {
        IEnumerable<AdminSlide> GetAdminSlides();
        IQueryable<Slide> GetSlidesByLayoutAsync(Layout layout);
    }
}

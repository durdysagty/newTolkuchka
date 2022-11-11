using newTolkuchka.Models;
using newTolkuchka.Services.Abstracts;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISlide : IActionFormFile<Slide>
    {
        IQueryable<Slide> GetSlidesByLayoutAsync(Layout layout);
        //Task AddSlideAsync(Slide slide, IFormFile imageru, IFormFile imageen, IFormFile imagetm, int width, int height);
        //Task EditSlideAsync(Slide slide, IFormFile imageru, IFormFile imageen, IFormFile imagetm, int width, int height);
        //string GetSlideImagePath(int id, LanVersion lanVersion);
    }
}

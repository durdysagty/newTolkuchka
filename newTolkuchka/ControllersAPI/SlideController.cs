using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SlideController : AbstractController<Slide, AdminSlide, ISlide>
    {
        private const int WIDTH = 600;
        private const int HEIGHT = 300;
        private const int LEFTWIDTH = 220;
        private const int LEFTHEIGHT = 300;
        public SlideController(IEntry entry, ISlide slide, ICacheClean cacheClean) : base(entry, Entity.Slide, slide, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Slide> Get(int id)
        {
            Slide slide = await _service.GetModelAsync(id);
            return slide;
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Slide slide, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(slide, _service.GetModels());
            if (isExist)
                return Result.Already;
            if (slide.Layout == Layout.Main)
                await _service.AddModelAsync(slide, images, WIDTH, HEIGHT);
            if (slide.Layout == Layout.Left)
                await _service.AddModelAsync(slide, images, LEFTWIDTH, LEFTHEIGHT);
            await AddActAsync(slide.Id, slide.Name);
            _cacheClean.CleanSlides();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Slide slide, [FromForm] IFormFile[] images)
        {
            bool isExist = _service.IsExist(slide, _service.GetModels().Where(x => x.Id != slide.Id));
            if (isExist)
                return Result.Already;
            if (slide.Layout == Layout.Main)
                await _service.EditModelAsync(slide, images, WIDTH, HEIGHT);
            if (slide.Layout == Layout.Left)
                await _service.EditModelAsync(slide, images, LEFTWIDTH, LEFTHEIGHT);
            await EditActAsync(slide.Id, slide.Name);
            _cacheClean.CleanSlides();
            return Result.Success;
        }

        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Slide slide = await _service.GetModelAsync(id);
            if (slide == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(slide.Id, slide);
            if (result == Result.Success)
                await DeleteActAsync(id, slide.Name);
            _cacheClean.CleanSlides();
            return result;
        }
    }
}
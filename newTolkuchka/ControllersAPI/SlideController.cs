using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SlideController : AbstractController
    {
        private const int WIDTH = 990;
        private const int HEIGHT = 450;
        private const int LEFTWIDTH = 220;
        private const int LEFTHEIGHT = 300;
        private readonly ISlide _slide;
        public SlideController(IEntry entry, ISlide slide) : base(entry, Entity.Slide)
        {
            _slide = slide;
        }

        [HttpGet("{id}")]
        public async Task<Slide> Get(int id)
        {
            Slide slide = await _slide.GetModelAsync(id);
            return slide;
        }
        [HttpGet]
        public IEnumerable<AdminSlide> Get()
        {
            IEnumerable<AdminSlide> slides = _slide.GetAdminSlides();
            return slides;
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Slide slide, [FromForm] IFormFile[] images)
        {
            bool isExist = _slide.IsExist(slide, _slide.GetModels());
            if (isExist)
                return Result.Already;
            if (slide.Layout == Layout.Main)
                await _slide.AddModelAsync(slide, images, WIDTH, HEIGHT);
            if (slide.Layout == Layout.Left)
                await _slide.AddModelAsync(slide, images, LEFTWIDTH, LEFTHEIGHT);
            await AddActAsync(slide.Id, slide.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Slide slide, [FromForm] IFormFile[] images)
        {
            bool isExist = _slide.IsExist(slide, _slide.GetModels().Where(x => x.Id != slide.Id));
            if (isExist)
                return Result.Already;
            if (slide.Layout == Layout.Main)
                await _slide.EditModelAsync(slide, images, WIDTH, HEIGHT);
            if (slide.Layout == Layout.Left)
                await _slide.EditModelAsync(slide, images, LEFTWIDTH, LEFTHEIGHT);
            await EditActAsync(slide.Id, slide.Name);
            return Result.Success;
        }

        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Slide slide = await _slide.GetModelAsync(id);
            if (slide == null)
                return Result.Fail;
            Result result = await _slide.DeleteModelAsync(slide.Id, slide);
            if (result == Result.Success)
                await DeleteActAsync(id, slide.Name);
            return result;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class ContentController : AbstractController<Content, IContent>
    {
        private readonly IContent _content;
        public ContentController(IEntry entry, IContent content) : base(entry, Entity.Content, content)
        {
            _content = content;
        }
        [HttpGet]
        public async Task<Content> Get()
        {
            Content content = await _content.GetContent();
            return content;
        }
        [HttpPut]
        public async Task<Result> Put(Content content)
        {
            await _content.EditContent(content);
            await EditActAsync(0, "Контент");
            return Result.Success;
        }
    }
}
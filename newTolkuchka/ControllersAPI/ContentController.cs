using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [AttributeUsage(AttributeTargets.All)]
    public class MyApiControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "api/content";
        public int? Order => 0;
        public string Name { get; set; } = string.Empty;
    }

    [MyApiController]
    [Authorize(Policy = "Level2")]
    public class ContentController : AbstractController<Content, Content, IContent>
    {
        public ContentController(IEntry entry, IContent content, ICacheClean cacheClean) : base(entry, Entity.Content, content, cacheClean)
        {
        }

        // in no ("edit") then AmbiguousMatchException. Abstract controller has same route.
        [HttpGet("edit")]
        public async Task<Content> Get()
        {
            Content content = await _service.GetContent();
            return content;
        }
        [HttpPut]
        public async Task<Result> Put(Content content)
        {
            await _service.EditContent(content);
            await EditActAsync(0, "Контент");
            return Result.Success;
        }
    }
}
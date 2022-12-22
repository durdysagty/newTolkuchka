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
    [Authorize(Policy = "Level3")]
    public class ContentController : AbstractController<Content, Content, IContent>
    {
        private readonly IContent _content;
        public ContentController(IEntry entry, IContent content) : base(entry, Entity.Content, content)
        {
            _content = content;
        }

        // in no ("edit") then AmbiguousMatchException. Abstract controller has same route.
        [HttpGet("edit")]
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
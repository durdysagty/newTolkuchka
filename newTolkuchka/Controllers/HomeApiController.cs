using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly IProduct _product;
        private readonly ISiteMap _siteMap;
        public HomeApiController(IProduct product, ISiteMap siteMap)
        {
            _product = product;
            _siteMap = siteMap;
        }

        [HttpGet]
        [Route($"a/{ConstantsService.PRODUCT}/{{id}}")]
        public async Task<IActionResult> Get(int id)
        {
            ApiProduct product = await _product.GetApiProductAsync(id);
            if (product == null)
            {
                return NotFound("Not Found");
            }
            return Ok(product);
        }

        // to update all sitemaps
        [HttpGet]
        [Route("a/sitemaprenew")]
        public async Task<IActionResult> SiteMapRenew(int id)
        {
            try
            {
                await _siteMap.RenewSiteMap();
                return Ok("Все файлы обновлены");
            }
            catch (Exception ex)
            {
                return Ok($"Произошла ошибка обновления: {ex}");
            }
        }
    }
}
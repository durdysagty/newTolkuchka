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
        public HomeApiController(IProduct product)
        {
            _product = product;
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
    }
}
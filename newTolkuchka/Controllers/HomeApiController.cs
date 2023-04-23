using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [ApiController]
    public class HomeApiController
    {
        private readonly IProduct _product;
        public HomeApiController(IProduct product)
        {
            _product = product;
        }

        [Route($"a/{ConstantsService.PRODUCT}/{{id}}")]
        public async Task<ApiProduct> Get(int id)
        {
            ApiProduct product = await _product.GetApiProductAsync(id);
            return product;
        }
    }
}
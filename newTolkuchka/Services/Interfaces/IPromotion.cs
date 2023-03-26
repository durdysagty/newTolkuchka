using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IPromotion : IActionFormFile<Promotion, AdminPromotion>
    {
        Task<int[]> GetProductsAsync(int id);
        Task AddPromotionProductsAsync(int id, IList<int> products);
    }
}

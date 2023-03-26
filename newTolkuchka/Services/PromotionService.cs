using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PromotionService : ServiceFormFile<Promotion, AdminPromotion>, IPromotion
    {
        public PromotionService(AppDbContext con, IPath path, IImage image, IStringLocalizer<Shared> localizer) : base(con, localizer, path, image, ConstantsService.LOCALMAXIMAGE)
        {
        }

        public async Task<int[]> GetProductsAsync(int id)
        {
            return await GetPromotionProducts(id).Select(x => x.ProductId).ToArrayAsync();
        }

        public async Task AddPromotionProductsAsync(int id, IList<int> products)
        {
            IList<PromotionProduct> promotionProducts = await GetPromotionProducts(id).ToListAsync();
            IList<PromotionProduct> toRemove = promotionProducts.Where(x => !products.Contains(x.ProductId)).ToList();
            foreach (var pp in toRemove)
            {
                _con.PromotionProducts.Remove(pp);
            }
            IList<int> toAdds = products.Where(x => !promotionProducts.Select(y => y.ProductId).Contains(x)).ToList();
            foreach (var toAdd in toAdds)
            {
                PromotionProduct promotionProduct = new()
                {
                    PromotionId = id,
                    ProductId = toAdd
                };
                await _con.PromotionProducts.AddAsync(promotionProduct);
            }
        }

        private IQueryable<PromotionProduct> GetPromotionProducts(int id)
        {
            return _con.PromotionProducts.Where(x => x.PromotionId == id);
        }
    }
}

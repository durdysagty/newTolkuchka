using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class PromotionController : AbstractController<Promotion, AdminPromotion, IPromotion>
    {
        private const int WIDTH = 450;
        private const int HEIGHT = 225;
        public PromotionController(IEntry entry, IPromotion promotion, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Promotion, promotion, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Promotion> Get(int id)
        {
            Promotion promotion = await _service.GetModelAsync(id);
            return promotion;
        }
        [HttpGet("products/{id}")]
        public async Task<int[]> GetProducts(int id)
        {
            return await _service.GetProductsAsync(id);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Promotion promotion, [FromForm] IFormFile[] images, [FromForm] IList<int> products)
        {
            bool isExist = _service.IsExist(promotion, _service.GetModels());
            if (isExist)
                return Result.Already;
            if (!products.Any())
                return Result.Fail;
            if ((promotion.Type is Tp.Discount or Tp.QuantityDiscount or Tp.SetDiscount or Tp.SpecialSetDiscount) && promotion.Volume is null or 0)
                return Result.Fail;
            if ((promotion.Type is Tp.ProductFree or Tp.Set) && promotion.SubjectId is null or 0)
                return Result.Fail;
            if (promotion.Type == Tp.QuantityDiscount && promotion.Quantity == null)
                promotion.Quantity = 1;
            await _service.AddModelAsync(promotion, images, WIDTH, HEIGHT);
            if (products.Any())
                await _service.AddPromotionProductsAsync(promotion.Id, products);
            await AddActAsync(promotion.Id, promotion.NameRu);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] EditPromotion editPromotion, [FromForm] IFormFile[] images, [FromForm] IList<int> products)
        {
            Promotion promotion = await _service.GetModelAsync(editPromotion.Id);
            if (promotion.Type != editPromotion.Type)
                return Result.Fail;
            promotion.Volume = editPromotion.Volume;
            promotion.Quantity = editPromotion.Quantity;
            promotion.SubjectId = editPromotion.SubjectId;
            promotion.NameRu = editPromotion.NameRu;
            promotion.NameEn = editPromotion.NameEn;
            promotion.NameTm = editPromotion.NameTm;
            promotion.DescRu = editPromotion.DescRu;
            promotion.DescEn = editPromotion.DescEn;
            promotion.DescTm = editPromotion.DescTm;
            promotion.NotInUse = editPromotion.NotInUse;
            bool isExist = _service.IsExist(promotion, _service.GetModels().Where(x => x.Id != promotion.Id));
            if (isExist)
                return Result.Already;
            if (!products.Any())
                return Result.Fail;
            if ((promotion.Type is Tp.Discount or Tp.QuantityDiscount or Tp.SetDiscount or Tp.SpecialSetDiscount) && promotion.Volume is null or 0)
                return Result.Fail;
            if (promotion.Type == Tp.QuantityDiscount && promotion.Quantity == null)
                promotion.Quantity = 1;
            await _service.EditModelAsync(promotion, images, WIDTH, HEIGHT);
            await _service.AddPromotionProductsAsync(promotion.Id, products);
            await EditActAsync(promotion.Id, promotion.NameRu, !promotion.NotInUse);
            _cacheClean.CleanIndexCategoriesPromotions();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Promotion promotion = await _service.GetModelAsync(id);
            if (promotion == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(promotion.Id, promotion);
            if (result == Result.Success)
                await DeleteActAsync(id, promotion.NameRu);
            _cacheClean.CleanIndexCategoriesPromotions();
            return result;
        }
    }
}
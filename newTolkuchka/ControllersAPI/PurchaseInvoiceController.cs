using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class PurchaseInvoiceController : AbstractController<PurchaseInvoice, AdminPurchaseInvoice, IPurchaseInvoice>
    {
        private readonly IPurchase _purchase;
        public PurchaseInvoiceController(IEntry entry, IPurchaseInvoice purchaseInvoice, ICacheClean cacheClean, IPurchase purchase) : base(entry, Entity.PurchaseInvoice, purchaseInvoice, cacheClean)
        {
            _purchase = purchase;
        }

        [HttpGet("{id}")]
        public async Task<PurchaseInvoice> Get(int id)
        {
            PurchaseInvoice PurchaseInvoice = await _service.GetModelAsync(id);
            return PurchaseInvoice;
        }
        //[HttpGet]
        //public IEnumerable<AdminPurchaseInvoice> Get()
        //{
        //    IEnumerable<AdminPurchaseInvoice> purchaseInvoices = _service.GetAdminPurchaseInvoices();
        //    return purchaseInvoices;
        //}
        [HttpGet("purchases/{id}")]
        public async Task<IEnumerable<AdminPurchase>> GetAdminPurchasesByPurchaseInvoiceId(int id)
        {
            return await _purchase.GetAdminPurchasesByPurchaseInvoiceId(id);
        }
        [HttpGet("store")]
        public IEnumerable<AdminStorePurchase> GetAdminStorePurchases([FromQuery] int[] ids, [FromQuery] int[] usedIds)
        {
            return _purchase.GetAdminStorePurchases(ids, usedIds);
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] PurchaseInvoice purchaseInvoice, [FromForm] string jsonPurchases)
        {
            IList<AdminPurchase> adminPurchases = JsonService.Deserialize<List<AdminPurchase>>(jsonPurchases);
            if (!adminPurchases.Any())
                return Result.Fail;
            purchaseInvoice.Date = DateTimeOffset.Now.ToUniversalTime();
            await _service.AddModelAsync(purchaseInvoice);
            await _purchase.AddPurchasesAsync(purchaseInvoice.Id, adminPurchases);
            await AddActAsync(purchaseInvoice.Id, CreateInvoiceName(purchaseInvoice));
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] PurchaseInvoice purchaseInvoice, [FromForm] string jsonPurchases)
        {
            IList<AdminPurchase> adminPurchases = JsonService.Deserialize<List<AdminPurchase>>(jsonPurchases);
            if (!adminPurchases.Any())
                return Result.Fail;
            // result will not to be saved, becouse result can be Fail
            Result result = await RemovePurchaseInvoicePurchases(purchaseInvoice.Id);
            if (result == Result.Fail)
                return result;
            await _purchase.AddPurchasesAsync(purchaseInvoice.Id, adminPurchases);
            _service.EditModel(purchaseInvoice);
            await EditActAsync(purchaseInvoice.Id, CreateInvoiceName(purchaseInvoice));
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            PurchaseInvoice purchaseInvoice = await _service.GetModelAsync(id);
            if (purchaseInvoice == null)
                return Result.Fail;
            Result result = await RemovePurchaseInvoicePurchases(purchaseInvoice.Id);
            if (result == Result.Fail)
                return result;
            await _purchase.SaveChangesAsync();
            result = await _service.DeleteModelAsync(purchaseInvoice.Id, purchaseInvoice);
            if (result == Result.Success)
                await DeleteActAsync(id, CreateInvoiceName(purchaseInvoice));
            return result;
        }

        private async Task<Result> RemovePurchaseInvoicePurchases(int purchaseInvoiceId)
        {
            // result will not to be saved, becouse result can be Fail
            Result result = await _purchase.RemovePurchaseInvoicePurchases(purchaseInvoiceId);
            return result;
        }

        private static string CreateInvoiceName(PurchaseInvoice purchaseInvoice)
        {
            return $"Приходный ордер #{purchaseInvoice.Id}";
        }
    }
}
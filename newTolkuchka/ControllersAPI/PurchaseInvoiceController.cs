using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Text.Json;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class PurchaseInvoiceController : AbstractController
    {
        private readonly IPurchaseInvoice _purchaseInvoice;
        private readonly IPurchase _purchase;
        public PurchaseInvoiceController(IEntry entry, IPurchaseInvoice PurchaseInvoice, IPurchase purchase) : base(entry, Entity.PurchaseInvoice)
        {
            _purchaseInvoice = PurchaseInvoice;
            _purchase = purchase;
        }

        [HttpGet("{id}")]
        public async Task<PurchaseInvoice> Get(int id)
        {
            PurchaseInvoice PurchaseInvoice = await _purchaseInvoice.GetModelAsync(id);
            return PurchaseInvoice;
        }
        [HttpGet]
        public IEnumerable<AdminPurchaseInvoice> Get()
        {
            IEnumerable<AdminPurchaseInvoice> purchaseInvoices = _purchaseInvoice.GetAdminPurchaseInvoices();
            return purchaseInvoices;
        }
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
            IList<AdminPurchase> adminPurchases = JsonSerializer.Deserialize<List<AdminPurchase>>(jsonPurchases, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (!adminPurchases.Any())
                return Result.Fail;
            purchaseInvoice.Date = DateTimeOffset.Now.ToUniversalTime();
            await _purchaseInvoice.AddModelAsync(purchaseInvoice);
            await _purchase.AddPurchasesAsync(purchaseInvoice.Id, adminPurchases);
            await AddActAsync(purchaseInvoice.Id, CreateInvoiceName(purchaseInvoice));
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] PurchaseInvoice purchaseInvoice, [FromForm] string jsonPurchases)
        {
            IList<AdminPurchase> adminPurchases = JsonSerializer.Deserialize<List<AdminPurchase>>(jsonPurchases, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (!adminPurchases.Any())
                return Result.Fail;
            // result will not to be saved, becouse result can be Fail
            Result result = await RemovePurchaseInvoicePurchases(purchaseInvoice.Id);
            if (result == Result.Fail)
                return result;
            await _purchase.AddPurchasesAsync(purchaseInvoice.Id, adminPurchases);
            _purchaseInvoice.EditModel(purchaseInvoice);
            await EditActAsync(purchaseInvoice.Id, CreateInvoiceName(purchaseInvoice));
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            PurchaseInvoice purchaseInvoice = await _purchaseInvoice.GetModelAsync(id);
            if (purchaseInvoice == null)
                return Result.Fail;
            Result result = await RemovePurchaseInvoicePurchases(purchaseInvoice.Id);
            if (result == Result.Fail)
                return result;
            await _purchase.SaveChangesAsync();
            result = await _purchaseInvoice.DeleteModelAsync(purchaseInvoice.Id, purchaseInvoice);
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
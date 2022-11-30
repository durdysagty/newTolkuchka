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
    public class InvoiceController : AbstractController
    {
        private readonly IInvoice _invoice;
        private readonly IOrder _order;
        public InvoiceController(IEntry entry, IInvoice Invoice, IOrder order) : base(entry, Entity.Invoice)
        {
            _invoice = Invoice;
            _order = order;
        }

        [HttpGet("{id}")]
        public async Task<Invoice> Get(int id)
        {
            Invoice invoice = await _invoice.GetModelAsync(id);
            return invoice;
        }
        [HttpGet("currency/{id}")]
        public AdminInvoice GetCurrencyIncluded(int id)
        {
            AdminInvoice invoice = _invoice.GetAdminInvoice(id);
            return invoice;
        }
        [HttpGet]
        public IEnumerable<AdminInvoice> Get()
        {
            IEnumerable<AdminInvoice> invoices = _invoice.GetAdminInvoices();
            return invoices;
        }
        [HttpGet("orders/{id}")]
        public async Task<IEnumerable<AdminOrder>> GetInvoiceAdminOrders(int id)
        {
            return await _order.GetAdminOrdersByInvoiceIdAsync(id);
        }
        [HttpGet("store/{id}")]
        public IEnumerable<AdminStoreOrder> GetInvoiceAdminStoreOrders(int id)
        {
            return _order.GetAdminStoreOrdersByInvoiceIdAsync(id);
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Invoice invoice, [FromForm] string jsonOrders)
        {
            IList<AdminOrder> adminOrders = JsonSerializer.Deserialize<List<AdminOrder>>(jsonOrders, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            await _order.CorrectOrdersAsync(invoice.Id, adminOrders);
            _invoice.EditModel(invoice);
            await EditActAsync(invoice.Id, CreateInvoiceName(invoice));
            return Result.Success;
        }
        [HttpPut("store/{id}")]
        public async Task<Result> PutOrdersPurchases(int id, [FromForm] IList<int?[]> orderPurchases, [FromForm] bool isDelivered, [FromForm] bool isPaid)
        {
            Invoice invoice = await _invoice.GetModelAsync(id);
            invoice.IsDelivered = isDelivered;
            invoice.IsPaid = isPaid;
            if (isPaid && invoice.PaidDate == null)
                invoice.PaidDate = DateTimeOffset.Now.ToUniversalTime();
            else if (!isPaid && invoice.PaidDate != null)
                invoice.PaidDate = null;
            foreach (int?[] op in orderPurchases)
            {
                Order order = await _order.GetModelAsync(op[0].Value);
                if (order == null)
                    return Result.Fail;
                if (order.PurchaseId != op[1])
                    order.PurchaseId = op[1];
            }
            _invoice.EditModel(invoice);
            await EditActAsync(invoice.Id, CreateInvoiceName(invoice));
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Invoice invoice = await _invoice.GetModelAsync(id);
            if (invoice == null)
                return Result.Fail;
            await DeleteActAsync(id, CreateInvoiceName(invoice));
            return Result.Success;
            //return result;
        }

        private static string CreateInvoiceName(Invoice invoice)
        {
            return $"Инвойс #{invoice.Id}";
        }
    }
}
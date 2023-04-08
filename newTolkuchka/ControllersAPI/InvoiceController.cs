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
    public class InvoiceController : AbstractController<Invoice, AdminInvoice, IInvoice>
    {
        private readonly IInvoice _invoice;
        private readonly IOrder _order;
        public InvoiceController(IEntry entry, IInvoice invoice, IOrder order) : base(entry, Entity.Invoice, invoice)
        {
            _invoice = invoice;
            _order = order;
        }

        [HttpGet("{id}")]
        public async Task<Invoice> Get(int id)
        {
            Invoice invoice = await _invoice.GetModelAsync(id);
            return invoice;
        }
        // used for InvoiceProcess in admin panel
        [HttpGet("currency/{id}")]
        public AdminInvoice GetCurrencyIncluded(int id)
        {
            AdminInvoice invoice = _invoice.GetAdminInvoices().FirstOrDefault(i => i.Id == id);
            return invoice;
        }
        // used for invoice & invoicePrint
        [HttpGet("orders/{id}")]
        public async Task<IEnumerable<AdminOrderExtended>> GetInvoiceAdminOrders(int id)
        {
            return await _order.GetAdminOrdersByInvoiceIdAsync(id);
        }
        [HttpGet("store/{id}")]
        public IEnumerable<AdminStoreOrder> GetInvoiceAdminStoreOrders(int id)
        {
            return _order.GetAdminStoreOrdersByInvoiceIdAsync(id);
        }
        //[HttpGet($"{ConstantsService.USER}/{{id}}")]
        //public async Task<ModelsFilters<UserInvoice>> GetUserInvoices(int id)
        //{
        //    IEnumerable<UserInvoice> invoices = await _invoice.GetUserInvoicesAsync(id);
        //    // we have to send ModelsFilters, b.o. Models page in admin, we can not send invoices directly, models page intendent to use paged/not paged models
        //    return new ModelsFilters<UserInvoice>()
        //    {
        //        Models = invoices,
        //        LastPage = 0,
        //        Pagination = null
        //    };
        //}
        [HttpPut]
        public async Task<Result> Put([FromForm] Invoice invoice, [FromForm] string jsonOrders)
        {
            IList<AdminOrderExtended> adminOrders = JsonService.Deserialize<List<AdminOrderExtended>>(jsonOrders);
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
        [Authorize(Policy = "Level3")]
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Invoice invoice = await _invoice.GetModelAsync(id);
            if (invoice == null)
                return Result.Fail;
            Result result = await _invoice.DeleteModelAsync(invoice.Id, invoice);
            if (result == Result.Success)
                await DeleteActAsync(id, CreateInvoiceName(invoice));
            return result;
            //return result;
        }

        private static string CreateInvoiceName(Invoice invoice)
        {
            return $"Инвойс #{invoice.Id}";
        }
    }
}
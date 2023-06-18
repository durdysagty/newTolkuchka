using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;

namespace newTolkuchka.Services
{
    public class InvoiceService : ServiceNoFile<Invoice, AdminInvoice>, IInvoice
    {
        private readonly IProduct _product;
        private readonly IOrder _order;
        public InvoiceService(AppDbContext con, IMemoryCache memoryCache, IProduct product, IOrder order, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, memoryCache, localizer, cacheClean)
        {
            _product = product;
            _order = order;
        }
        public async Task CreateInvoice(int? userId, CartOrder[] cartOrders, DeliveryData deliveryData, Guid customerGuidId)
        {

            Invoice invoice = new()
            {
                Date = DateTimeOffset.Now.ToUniversalTime(),
                Buyer = deliveryData.Name,
                InvoiceAddress = deliveryData.Address,
                InvoiceEmail = deliveryData.Email,
                InvoicePhone = deliveryData.Phone,
                CurrencyRate = CurrencyService.Currency.RealRate,
                CurrencyId = CurrencyService.Currency.Id,
                UserId = userId,
                CustomerGuidId = customerGuidId,
                Language = CultureProvider.CurrentCulture
            };
            await AddModelAsync(invoice, true);
            decimal sum = await _order.CreateOrders(invoice.Id, cartOrders);
            if (sum < ConstantsService.DELIVERYFREE)
                invoice.DeliveryCost = ConstantsService.DELIVERYPRICE;
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<UserInvoice>> GetUserInvoicesAsync(int userId)
        {
            IEnumerable<Invoice> invoices = GetModels().Where(i => i.UserId == userId).OrderByDescending(i => i.Id).Include(i => i.Orders).Include(i => i.Currency);
            #region 
            //IEnumerable<UserInvoice> userInvoices = invoices.OrderByDescending(i => i.Id).Select(i => new UserInvoice
            //{
            //    Id = i.Id,
            //    Recipient = $"{i.Buyer}, {i.InvoiceAddress}, {i.InvoicePhone}{(i.InvoiceEmail != null ? $", {i.InvoiceEmail}" : "")}",
            //    Date = i.Date.DateTime,
            //    Amount = i.Orders.Select(o => o.OrderPrice).Sum() + i.DeliveryCost,
            //    Currency = i.Currency,
            //    Payment = i.IsPaid,
            //    Delivery = i.IsDelivered,
            //    DeliveryCost = i.DeliveryCost,
            //    UserOrders = i.Orders.DistinctBy(o => o.ProductId).Select(async o => new UserOrder
            //    {
            //        ProductName = IProduct.GetProductName(_product.GetFullProductAsync(o.Id).Result, null),
            //        Warranty = CultureProvider.GetLocalName(o.Product.Warranty.NameRu, o.Product.Warranty.NameEn, o.Product.Warranty.NameTm),
            //        Price = o.OrderPrice,
            //        Quantity = i.Orders.Count(c => c.ProductId == o.ProductId)
            //    }).ToArray()
            //});
            #endregion
            IList<UserInvoice> userInvoices = new List<UserInvoice>();
            foreach (Invoice i in invoices)
            {
                UserInvoice userInvoice = new ()
                {
                    Id = i.Id,
                    Recipient = $"{i.Buyer}, {i.InvoiceAddress}, {i.InvoicePhone}{(i.InvoiceEmail != null ? $", {i.InvoiceEmail}" : "")}",
                    Date = i.Date,
                    Amount = i.Orders.Select(o => o.OrderPrice).Sum() + i.DeliveryCost,
                    Currency = i.Currency.CodeName,
                    IsPaid = i.IsPaid,
                    IsDelivered = i.IsDelivered,
                    DeliveryCost = i.DeliveryCost,
                    Orders = new Collection<UserOrder>()
                };
                foreach (Order o in i.Orders.DistinctBy(o => o.ProductId))
                {
                    Product p = await _product.GetFullProductAsync(o.ProductId);
                    UserOrder userOrder = new()
                    {
                        ProductName = IProduct.GetProductNameCounted(p, null),
                        Warranty = CultureProvider.GetLocalName(p.Model.Warranty.NameRu, p.Model.Warranty.NameEn, p.Model.Warranty.NameTm),
                        Price = o.OrderPrice,
                        Quantity = i.Orders.Count(c => c.ProductId == o.ProductId)
                    };
                    userInvoice.Orders.Add(userOrder);
                }
                userInvoices.Add(userInvoice);
            }
            return userInvoices;
        }

        public IEnumerable<AdminInvoice> GetAdminInvoices()
        {
            IEnumerable<AdminInvoice> invoices = GetModels().Select(x => new AdminInvoice
            {
                Id = x.Id,
                Date = x.Date,
                User = x.UserId == null ? null : x.User.Email,
                Buyer= x.Buyer,
                Address = x.InvoiceAddress,
                Phone = x.InvoicePhone,
                Language = x.Language,
                CurrencyCodeName = x.Currency.CodeName,
                CurrencyRate = x.CurrencyRate,
                //Orders = x.Orders.Count,
                DeliveryCost= x.DeliveryCost,
                IsPaid = x.IsPaid,
                IsDelivered= x.IsDelivered
            }).OrderByDescending(x => x.Id);
            return invoices;
        }
    }
}

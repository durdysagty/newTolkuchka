using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;

namespace newTolkuchka.Services
{
    public class InvoiceService : ServiceNoFile<Invoice>, IInvoice
    {
        private readonly IProduct _product;
        public InvoiceService(AppDbContext con, IProduct product, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
            _product = product;
        }

        public AdminInvoice GetAdminInvoice(int id)
        {
            return GetModels().Where(i => i.Id == id).Select(x => new AdminInvoice
            {
                Id = x.Id,
                Date = x.Date,
                User = x.UserId == null ? null : x.User.Email,
                Buyer = x.Buyer,
                Address = x.InvoiceAddress,
                Phone = x.InvoicePhone,
                Language = x.Language,
                CurrencyCodeName = x.Currency.CodeName,
                DeliveryCost = x.DeliveryCost,
                IsDelivered= x.IsDelivered,
                IsPaid = x.IsPaid
            }).FirstOrDefault();
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
                    Currency = i.Currency,
                    Payment = i.IsPaid,
                    Delivery = i.IsDelivered,
                    DeliveryCost = i.DeliveryCost,
                    UserOrders = new Collection<UserOrder>()
                };
                foreach (Order o in i.Orders.DistinctBy(o => o.ProductId))
                {
                    Product p = await _product.GetFullProductAsync(o.ProductId);
                    UserOrder userOrder = new()
                    {
                        ProductName = IProduct.GetProductName(p, null),
                        Warranty = CultureProvider.GetLocalName(p.Warranty.NameRu, p.Warranty.NameEn, p.Warranty.NameTm),
                        Price = o.OrderPrice,
                        Quantity = i.Orders.Count(c => c.ProductId == o.ProductId)
                    };
                    userInvoice.UserOrders.Add(userOrder);
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
                CurrencyCodeName = x.Currency.CodeName,
                CurrencyRate = x.CurrencyRate,
                Orders = x.Orders.Count,
                IsDelivered= x.IsDelivered,
                IsPaid= x.IsPaid
            }).OrderByDescending(x => x.Id);
            return invoices;
        }
    }
}

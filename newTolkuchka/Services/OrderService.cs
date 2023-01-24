using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Org.BouncyCastle.Asn1.X509;

namespace newTolkuchka.Services
{
    public class OrderService : ServiceNoFile<Order, AdminOrder>, IOrder
    {

        private readonly IProduct _product;

        public OrderService(AppDbContext con, IStringLocalizer<Shared> localizer, IProduct product) : base(con, localizer)
        {
            _product = product;
        }

        public void CreateCartOrders(IList<CartOrder> orders)
        {
            foreach (var order in orders)
            {
                Product product = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new[] { order.Id } } }).FirstOrDefault();
                if (product != null)
                {
                    order.ProductName = IProduct.GetProductNameCounted(product);
                    order.Price = IProduct.GetConvertedPrice(product.NewPrice != null ? (decimal)product.NewPrice : product.Price);
                    order.Amount = order.Price * order.Quantity;
                    order.Image = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", product.Id);
                }
                else
                    orders.Remove(order);
            }

        }

        public IEnumerable<Order> GetOrdersByInvoiceId(int id)
        {
            return GetModels().Where(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<AdminOrder>> GetAdminOrdersByInvoiceIdAsync(int id)
        {
            IEnumerable<Order> orders = GetOrdersByInvoiceId(id);
            IList<AdminOrder> adminOrders = new List<AdminOrder>();
            foreach (Order o in orders)
            {
                AdminOrder adminOrder = adminOrders.FirstOrDefault(ap => ap.ProductId == o.ProductId);
                if (adminOrder == null)
                {
                    adminOrder = new()
                    {
                        ProductId = o.ProductId,
                        Name = IProduct.GetProductNameCounted(await _product.GetFullProductAsync(o.ProductId)),
                        OrderPrice = o.OrderPrice,
                        SerialNumbers = o.Purchase?.SerialNumber,
                        Quantity = 1
                    };
                    adminOrders.Add(adminOrder);
                }
                else
                {
                    adminOrder.SerialNumbers += o.Purchase?.SerialNumber != null ? $" {o.Purchase.SerialNumber}": null;
                    adminOrder.Quantity++;
                }
            }
            return adminOrders;
        }

        public IEnumerable<AdminStoreOrder> GetAdminStoreOrdersByInvoiceIdAsync(int id)
        {
            IEnumerable<AdminStoreOrder> adminStoreOrders = GetOrdersByInvoiceId(id).Select(o => new AdminStoreOrder
            {
                Id= o.Id,
                Name = IProduct.GetProductNameCounted(_product.GetFullProductAsync(o.ProductId).Result),
                OrderPrice = o.OrderPrice,
                ProductId= o.ProductId,
                PurchaseId = o.PurchaseId
            });
            return adminStoreOrders;
        }

        public async Task CorrectOrdersAsync(int invoiceId, IList<AdminOrder> adminOrders)
        {
            IEnumerable<Order> orders = GetOrdersByInvoiceId(invoiceId);
            IEnumerable<Order> ordersToDel = orders.Where(o => !adminOrders.Select(a => a.ProductId).Distinct().Contains(o.ProductId));
            if (ordersToDel.Any())
                foreach (Order o in ordersToDel)
                    await DeleteModelAsync(o.Id, o);
            if (adminOrders.Any())
                foreach (AdminOrder ao in adminOrders)
                {
                    IEnumerable<Order> ordersToCheck = orders.Where(o => o.ProductId == ao.ProductId);
                    if (!ordersToCheck.Any())
                    {
                        for (int i = 0; i < ao.Quantity; i++)
                        {
                            Order order = new()
                            {
                                ProductId = ao.ProductId,
                                OrderPrice = ao.OrderPrice,
                                InvoiceId = invoiceId,
                            };
                            await AddModelAsync(order);
                        }
                        continue;
                    }
                    if (ordersToCheck.Count() > ao.Quantity)
                    {
                        IEnumerable<Order> ordersOutOfQuantity = ordersToCheck.OrderByDescending(x => x.Id).Take(ordersToCheck.Count() - ao.Quantity);
                        foreach (Order o in ordersOutOfQuantity)
                            await DeleteModelAsync(o.Id, o);
                        ordersToCheck = ordersToCheck.Except(ordersOutOfQuantity);
                    }
                    foreach (Order o in ordersToCheck)
                    {
                        o.OrderPrice = ao.OrderPrice;
                    }
                    if (ordersToCheck.Count() < ao.Quantity)
                    {
                        for (int i = 0; i < ao.Quantity - ordersToCheck.Count() ; i++)
                        {
                            Order order = new()
                            {
                                ProductId = ao.ProductId,
                                OrderPrice = ao.OrderPrice,
                                InvoiceId = invoiceId,
                            };
                            await AddModelAsync(order);
                        }
                    }
                }
        }
    }
}

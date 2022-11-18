using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class OrderService : ServiceNoFile<Order>, IOrder
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
                Product product = _product.GetFullProducts(null, null, null, null, null, new[] { order.Id }).FirstOrDefault();
                if (product != null)
                {
                    order.ProductName = IProduct.GetProductName(product);
                    order.Price = IProduct.GetConvertedPrice(product.NewPrice != null ? (decimal)product.NewPrice : product.Price);
                    order.Amount = order.Price * order.Quantity;
                    order.Image = PathService.GetImageRelativePath(ConstantsService.PRODUCT + "/small", product.Id);
                }
                else
                    orders.Remove(order);
            }

        }
    }
}

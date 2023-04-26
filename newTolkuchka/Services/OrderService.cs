using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Models.Migs;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class OrderService : ServiceNoFile<Order, AdminOrderExtended>, IOrder
    {

        private readonly IProduct _product;

        public OrderService(AppDbContext con, IStringLocalizer<Shared> localizer, IProduct product) : base(con, localizer)
        {
            _product = product;
        }

        public void CreateCartOrders(IList<CartOrder> cartOrders)
        {
            List<Promotion> setDiscountsAll = new();
            List<Promotion> setDiscountsNotAll = new();
            List<Promotion> specialSetDiscountsAll = new();
            List<Promotion> specialSetDiscountsNotAll = new();
            List<(int, decimal?, string)> specialSetDiscountSubjectIdVolumes = new();
            foreach (var cartOrder in cartOrders)
            {
                Product product = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { cartOrder.Id } } }).FirstOrDefault();
                if (product != null)
                {
                    Promotion quantityFree = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.QuantityFree)?.Promotion;
                    Promotion productFree = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.ProductFree)?.Promotion;
                    Promotion set = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Set)?.Promotion;
                    Promotion setDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.SetDiscount)?.Promotion;
                    // we need to check is all products of setDiscount are presented in this cart, if not setDiscount are not actual
                    if (setDiscount != null)
                        if (!setDiscountsAll.Any(sd => sd.Id == setDiscount.Id))
                        {
                            if (setDiscountsNotAll.Any(sd => sd.Id == setDiscount.Id))
                            {
                                setDiscount = null;
                            }
                            else
                            {
                                if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, setDiscount.Id } }).ToArray().All(p => cartOrders.Any(ca => ca.Id == p.Id)))
                                {
                                    setDiscountsAll.Add(setDiscount);
                                }
                                else
                                {
                                    setDiscountsNotAll.Add(setDiscount);
                                    setDiscount = null;
                                }
                            }
                        }
                    Promotion quantityDiscount = null;
                    if (setDiscount == null)
                        quantityDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.QuantityDiscount)?.Promotion;
                    Promotion specialSetDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.SpecialSetDiscount)?.Promotion;
                    // we need to check is all products of specialSetDiscount are presented in this cart, if not the promo are not actual
                    if (specialSetDiscount != null)
                        if (!specialSetDiscountsAll.Any(ssd => ssd.Id == specialSetDiscount.Id))
                        {
                            if (!specialSetDiscountsNotAll.Any(ssd => ssd.Id == specialSetDiscount.Id))
                            {
                                if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, specialSetDiscount.Id } }).ToArray().All(p => cartOrders.Any(ca => ca.Id == p.Id)))
                                {
                                    specialSetDiscountsAll.Add(specialSetDiscount);
                                    specialSetDiscountSubjectIdVolumes.Add(((int)specialSetDiscount.SubjectId, specialSetDiscount.Volume, $"<a href=\"{PathService.GetModelUrl(ConstantsService.PROMOTION, specialSetDiscount.Id)}\">{CultureProvider.GetLocalName(specialSetDiscount.NameRu, specialSetDiscount.NameEn, specialSetDiscount.NameTm)}</a>"));
                                }
                                else
                                {
                                    specialSetDiscountsNotAll.Add(specialSetDiscount);
                                }
                            }
                        }
                    cartOrder.ProductName = IProduct.GetProductNameCounted(product);
                    // we can not include check quantityDiscountPromotion to GetOrderPrice becouse we need quantityDiscountPromotion's properties
                    // also we need price without quantityDiscountPromotion
                    // setDiscount supposed to be more valurable then other discounts
                    cartOrder.Price = setDiscount != null ? IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * setDiscount.Volume / 100)) : quantityDiscount != null && cartOrder.Quantity >= quantityDiscount.Quantity ? IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * quantityDiscount.Volume / 100)) : IProduct.GetOrderPrice(product);
                    cartOrder.Amount = cartOrder.Price * cartOrder.Quantity;
                    cartOrder.Image = PathService.GetImageRelativePath($"{ConstantsService.PRODUCT}/small", product.Id);
                    cartOrder.ImageVersion = product.Version;
                    cartOrder.DiscountQuantity = quantityDiscount?.Quantity;
                    cartOrder.QuantityPrice = quantityDiscount == null ? null : IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * quantityDiscount.Volume / 100));
                    cartOrder.RegularPrice = quantityDiscount == null ? null : IProduct.GetOrderPrice(product);
                    cartOrder.FreeQuantity = quantityFree?.Quantity;
                    cartOrder.FreeProductQuantity = productFree?.Quantity;
                    cartOrder.FreeProductName = productFree == null ? null : IProduct.GetProductNameCounted(_product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { (int)productFree.SubjectId } } }).FirstOrDefault());
                    cartOrder.SetId = set != null && !cartOrders.Any(c => c.SetId == set.Id) && _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, set.Id } }).ToArray().All(p => cartOrders.Any(ca => ca.Id == p.Id)) ? set.Id : null;
                    cartOrder.SetFreeProductName = cartOrder.SetId != null ? IProduct.GetProductNameCounted(_product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { (int)set.SubjectId } } }).FirstOrDefault()) : null;
                }
                else
                    cartOrders.Remove(cartOrder);
            }
            // specialSetDiscount's subject discount supposed to be more valurable then other discounts
            // also we need check all orders is any of them is subjected, becouse of the subject could be first product in the cart
            if (specialSetDiscountSubjectIdVolumes.Any())
                foreach ((int, decimal?, string) ssdsiv in specialSetDiscountSubjectIdVolumes)
                {
                    CartOrder cartOrder = cartOrders.Where(co => co.Id == ssdsiv.Item1).FirstOrDefault();
                    if (cartOrder != null)
                    {
                        Product product = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { cartOrder.Id } } }).FirstOrDefault();
                        if (cartOrder.Quantity > 1)
                        {
                            cartOrder.Quantity--;
                            cartOrder.Amount = cartOrder.Quantity * cartOrder.Price;
                            CartOrder subjCartOrder = new()
                            {
                                Id = cartOrder.Id,
                                ProductName = $"{cartOrder.ProductName} - {CultureProvider.GetLocalName($"Специальная цена по акции - {ssdsiv.Item3}", $"Special \"{ssdsiv.Item3}\" promotion price", $"\"{ssdsiv.Item3}\" aksiýa bahasy")}",
                                Price = IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * ssdsiv.Item2 / 100)),
                                Quantity = 1,
                                Amount = IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * ssdsiv.Item2 / 100)),
                                Image = cartOrder.Image,
                                Subjected = true
                            };
                            cartOrders.Add(subjCartOrder);
                        }
                        else
                        {
                            cartOrder.ProductName = $"{cartOrder.ProductName} - {CultureProvider.GetLocalName($"Специальная цена по акции - {ssdsiv.Item3}", $"Special \"{ssdsiv.Item3}\" promotion price", $"\"{ssdsiv.Item3}\" aksiýa bahasy")}";
                            cartOrder.Price = IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * ssdsiv.Item2 / 100));
                            cartOrder.Amount = IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * ssdsiv.Item2 / 100));
                            cartOrder.Subjected = true;
                        }
                    }
                }
        }

        public async Task<decimal> CreateOrders(int invoiceId, IList<CartOrder> cartOrders)
        {
            decimal sum = 0;
            List<Promotion> setDiscountsAll = new();
            List<Promotion> setDiscountsNotAll = new();
            List<Promotion> specialSetDiscountsAll = new();
            List<Promotion> specialSetDiscountsNotAll = new();
            List<(int, decimal?)> specialSetDiscountSubjectIdVolumes = new();
            List<Order> orders = new();
            foreach (CartOrder cartOrder in cartOrders)
            {
                // may be optimize to select prodcut with Promotions only, not full version of product
                Product product = _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new[] { cartOrder.Id } } }).FirstOrDefault();
                if (product != null)
                {
                    Promotion quantityFree = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.QuantityFree)?.Promotion;
                    Promotion productFree = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.ProductFree)?.Promotion;
                    Promotion set = cartOrder.SetId != null ? product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.Set)?.Promotion : null;
                    Promotion setDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.SetDiscount)?.Promotion;
                    // we need to check is all products of setDiscount are presented in this cart, if not setDiscount are not actual
                    if (setDiscount != null)
                        if (!setDiscountsAll.Any(sd => sd.Id == setDiscount.Id))
                        {
                            if (setDiscountsNotAll.Any(sd => sd.Id == setDiscount.Id))
                            {
                                setDiscount = null;
                            }
                            else
                            {
                                if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, setDiscount.Id } }).ToArray().All(p => cartOrders.Any(ca => ca.Id == p.Id)))
                                {
                                    setDiscountsAll.Add(setDiscount);
                                }
                                else
                                {
                                    setDiscountsNotAll.Add(setDiscount);
                                    setDiscount = null;
                                }
                            }
                        }
                    Promotion quantityDiscount = null;
                    if (setDiscount == null)
                        quantityDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.QuantityDiscount)?.Promotion;
                    Promotion specialSetDiscount = product.PromotionProducts.FirstOrDefault(pp => pp.Promotion.Type == Tp.SpecialSetDiscount)?.Promotion;
                    // we need to check is all products of specialSetDiscount are presented in this cart, if not the promo are not actual
                    if (specialSetDiscount != null)
                        if (!specialSetDiscountsAll.Any(ssd => ssd.Id == specialSetDiscount.Id))
                        {
                            if (!specialSetDiscountsNotAll.Any(ssd => ssd.Id == specialSetDiscount.Id))
                            {
                                if (_product.GetModels(new Dictionary<string, object>() { { ConstantsService.PROMOTION, specialSetDiscount.Id } }).ToArray().All(p => cartOrders.Any(ca => ca.Id == p.Id)))
                                {
                                    specialSetDiscountsAll.Add(specialSetDiscount);
                                    specialSetDiscountSubjectIdVolumes.Add(((int)specialSetDiscount.SubjectId, specialSetDiscount.Volume));
                                }
                                else
                                {
                                    specialSetDiscountsNotAll.Add(specialSetDiscount);
                                }
                            }
                        }
                    for (int i = 0; i < cartOrder.Quantity; i++)
                    {
                        Order order = new()
                        {
                            ProductId = product.Id,
                            InvoiceId = invoiceId,
                            OrderPrice = setDiscount != null ? IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * setDiscount.Volume / 100)) : quantityDiscount != null && cartOrder.Quantity >= quantityDiscount.Quantity ? IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * quantityDiscount.Volume / 100)) : IProduct.GetOrderPrice(product)
                        };
                        orders.Add(order);
                        //await AddModelAsync(order);
                        // if sum more than deliveryfree it is not nessasary to check sum
                        if (sum < ConstantsService.DELIVERYFREE)
                            sum += cartOrder.Price;
                    }
                    if (quantityFree != null && cartOrder.Quantity >= quantityFree.Quantity)
                    {
                        Order order = new()
                        {
                            ProductId = product.Id,
                            InvoiceId = invoiceId,
                            OrderPrice = 0
                        };
                        orders.Add(order);
                        //await AddModelAsync(order);
                    }
                    if (productFree != null && cartOrder.Quantity >= productFree.Quantity)
                    {
                        Order order = new()
                        {
                            ProductId = (int)productFree.SubjectId,
                            InvoiceId = invoiceId,
                            OrderPrice = 0
                        };
                        orders.Add(order);
                        //await AddModelAsync(order);
                    }
                    if (set != null)
                    {
                        Order order = new()
                        {
                            ProductId = (int)set.SubjectId,
                            InvoiceId = invoiceId,
                            OrderPrice = 0
                        };
                        orders.Add(order);
                        //await AddModelAsync(order);
                    }
                }
            }
            // specialSetDiscount's subject discount supposed to be more valurable then other discounts
            // also we need check all orders is any of them is subjected, becouse of the subject could be first product in the cart
            if (specialSetDiscountSubjectIdVolumes.Any())
                foreach ((int, decimal?) ssdsiv in specialSetDiscountSubjectIdVolumes)
                {
                    Order order = orders.Where(co => co.ProductId == ssdsiv.Item1).FirstOrDefault();
                    if (order != null)
                    {
                        Product product = _product.GetModels(new Dictionary<string, object>() { { ConstantsService.PRODUCT, new int[1] { order.ProductId } } }).FirstOrDefault();
                        order.OrderPrice = IProduct.GetConvertedPrice((decimal)(product.Price - product.Price * ssdsiv.Item2 / 100));
                    }
                }
            // we remove AddModelAsync from scope above becouse of specialSetDiscountSubjectIdVolumes
            foreach (Order order in orders)
            {
                await AddModelAsync(order);
            }
            return sum;
        }

        public IEnumerable<Order> GetOrdersByInvoiceId(int id)
        {
            return GetModels().Where(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<AdminOrderExtended>> GetAdminOrdersByInvoiceIdAsync(int id)
        {
            IEnumerable<Order> orders = GetOrdersByInvoiceId(id);
            IList<AdminOrderExtended> adminOrders = new List<AdminOrderExtended>();
            int i = 1;
            foreach (Order o in orders)
            {
                AdminOrderExtended adminOrder = adminOrders.FirstOrDefault(ap => ap.ProductId == o.ProductId && ap.OrderPrice == o.OrderPrice);
                if (adminOrder == null)
                {
                    adminOrder = new()
                    {
                        Id = i++,
                        ProductId = o.ProductId,
                        Name = IProduct.GetProductNameCounted(await _product.GetFullProductAsync(o.ProductId)),
                        OrderPrice = o.OrderPrice,
                        SerialNumbers = o.Purchase?.SerialNumber,
                        Warranty = CultureProvider.GetLocalName(o.Product.Model.Warranty.NameRu, o.Product.Model.Warranty.NameEn, o.Product.Model.Warranty.NameTm),
                        Quantity = 1
                    };
                    adminOrders.Add(adminOrder);
                }
                else
                {
                    adminOrder.SerialNumbers += o.Purchase?.SerialNumber != null ? $" {o.Purchase.SerialNumber}" : null;
                    adminOrder.Quantity++;
                }
            }
            return adminOrders;
        }

        public IEnumerable<AdminStoreOrder> GetAdminStoreOrdersByInvoiceIdAsync(int id)
        {
            IEnumerable<AdminStoreOrder> adminStoreOrders = GetOrdersByInvoiceId(id).Select(o => new AdminStoreOrder
            {
                Id = o.Id,
                Name = IProduct.GetProductNameCounted(_product.GetFullProductAsync(o.ProductId).Result),
                OrderPrice = o.OrderPrice,
                ProductId = o.ProductId,
                PurchaseId = o.PurchaseId
            });
            return adminStoreOrders;
        }

        public async Task CorrectOrdersAsync(int invoiceId, IList<AdminOrderExtended> adminOrders)
        {
            IEnumerable<Order> orders = GetOrdersByInvoiceId(invoiceId);
            IEnumerable<Order> ordersToDel = orders.Where(o => !adminOrders.Any(ao => ao.ProductId == o.ProductId && ao.OrderPrice == o.OrderPrice));
            if (ordersToDel.Any())
                foreach (Order o in ordersToDel)
                    await DeleteModelAsync(o.Id, o);
            if (adminOrders.Any())
                foreach (AdminOrderExtended ao in adminOrders)
                {
                    IEnumerable<Order> ordersToCheck = orders.Where(o => o.ProductId == ao.ProductId && ao.OrderPrice == o.OrderPrice);
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
                    if (ordersToCheck.Count() < ao.Quantity)
                    {
                        for (int i = 0; i < ao.Quantity - ordersToCheck.Count(); i++)
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

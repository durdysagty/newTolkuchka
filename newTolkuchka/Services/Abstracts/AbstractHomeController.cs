#region using
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using F = System.IO.File;
#endregion
namespace newTolkuchka.Services.Abstracts
{
    public class AbstractHomeController : Controller
    {
        #region staff
        private protected readonly IStringLocalizer<Shared> _localizer;
        private protected readonly IBreadcrumbs _breadcrumbs;
        private protected readonly IPath _path;
        private protected readonly ICategory _category;
        private protected readonly IBrand _brand;
        private protected readonly IPromotion _promotion;
        private protected readonly IModel _model;
        private protected readonly IProduct _product;
        private protected readonly ISlide _slide;
        private protected readonly IActionNoFile<Heading, Heading> _heading;
        private protected readonly IArticle _article;
        private protected readonly IUser _user;
        private protected readonly ICustomerGuid _customerGuid;
        private protected readonly IInvoice _invoice;
        private protected readonly IOrder _order;
        private protected readonly ILogin _login;
        private protected readonly IActionNoFile<Currency, AdminCurrency> _currency;
        private protected readonly IMemoryCache _memoryCache;
        private protected readonly ICrypto _crypto;
        // to remove
        private protected readonly ILogger<AbstractHomeController> _logger;

        public AbstractHomeController(IStringLocalizer<Shared> localizer, IBreadcrumbs breadcrumbs, IPath path, ICategory category, IBrand brand, IPromotion promotion, IModel model, IProduct product, ISlide slide, IActionNoFile<Heading, Heading> heading, IArticle article, IUser user, ICustomerGuid customerGuid, IInvoice invoice, IOrder order, ILogin login, IActionNoFile<Currency, AdminCurrency> currency, IMemoryCache memoryCache, ICrypto crypto, ILogger<AbstractHomeController> logger)
        {
            _localizer = localizer;
            _breadcrumbs = breadcrumbs;
            _path = path;
            _category = category;
            _brand = brand;
            _promotion = promotion;
            _model = model;
            _product = product;
            _slide = slide;
            _heading = heading;
            _article = article;
            _user = user;
            _customerGuid = customerGuid;
            _order = order;
            _invoice = invoice;
            _login = login;
            _currency = currency;
            _memoryCache = memoryCache;
            _crypto = crypto;
            _logger = logger;
        }
        #endregion

        private protected const int selectedCategoryId = 11;
        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexSelectedCatProducts(int count)
        {
            IEnumerable<int> mobileIds = _model.GetModels(new Dictionary<string, object>() { { ConstantsService.CATEGORY, selectedCategoryId } }).OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> mobileUIProducts = new();
            foreach (int m in mobileIds)
            {
                Product[] mobileProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => !p.NotInUse).ToArrayAsync();
                mobileUIProducts.Add(_product.GetUIProduct(mobileProducts).ToList());
            }
            return mobileUIProducts;
        }

        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexNewProducts(int count)
        {
            IEnumerable<int> newModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.CategoryId != selectedCategoryId && m.Products.Any(p => p.IsNew && !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> newUIProducts = new();
            foreach (int m in newModelIds)
            {
                Product[] newProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsNew && !p.NotInUse).ToArrayAsync();
                newUIProducts.Add(_product.GetUIProduct(newProducts).ToList());
            }
            return newUIProducts;
        }

        private protected async Task<List<IEnumerable<UIProduct>>> GetIndexRecProducts(int count)
        {
            IEnumerable<int> recModelIds = _model.GetModels().OrderByDescending(m => m.Id).Where(m => !m.Category.NotInUse && m.Products.Any(p => p.IsRecommended && !p.NotInUse)).Take(count).Select(m => m.Id);
            List<IEnumerable<UIProduct>> recUIProducts = new();
            foreach (int m in recModelIds)
            {
                Product[] recProducts = await _product.GetFullModels(new Dictionary<string, object>() { { ConstantsService.MODEL, m } }).Where(p => p.IsRecommended && !p.NotInUse).ToArrayAsync();
                recUIProducts.Add(_product.GetUIProduct(recProducts).ToList());
            }
            return recUIProducts;
        }
    }
}
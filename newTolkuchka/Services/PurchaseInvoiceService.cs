using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PurchaseInvoiceService : ServiceNoFile<PurchaseInvoice, AdminPurchaseInvoice>, IPurchaseInvoice
    {
        public PurchaseInvoiceService(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
        }
    }
}
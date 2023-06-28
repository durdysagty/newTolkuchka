using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class CurrencyService : ServiceNoFile<Currency, AdminCurrency>
    {
        public static Currency Currency { get; set; }
        public CurrencyService(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
            Currency ??= GetModels().AsNoTracking().FirstOrDefault(c => c.Id == 2);
        }
    }
}

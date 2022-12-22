using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;

namespace newTolkuchka.Services
{
    public class CurrencyService : ServiceNoFile<Currency, AdminCurrency>
    {
        public static Currency Currency { get; set; }
        public CurrencyService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
            Currency = GetModels().AsNoTracking().FirstOrDefault(c => c.Id == 2);
        }
    }
}

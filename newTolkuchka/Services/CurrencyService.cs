using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Services.Abstracts;

namespace newTolkuchka.Services
{
    public class CurrencyService : ServiceNoFile<Currency>
    {
        public static Currency Currency { get; set; }
        public CurrencyService(AppDbContext con) : base(con)
        {
            Currency = GetModels().AsNoTracking().FirstOrDefault(c => c.Id == 2);
        }
    }
}

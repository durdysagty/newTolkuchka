using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecsValueModService : ServiceNoFile<SpecsValueMod, AdminSpecsValueMod>, ISpecsValueMod
    {
        public SpecsValueModService(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
        }
    }
}

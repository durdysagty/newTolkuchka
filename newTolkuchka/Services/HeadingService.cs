using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;

namespace newTolkuchka.Services
{
    public class HeadingService : ServiceNoFile<Heading, Heading>
    {
        public HeadingService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }
    }
}

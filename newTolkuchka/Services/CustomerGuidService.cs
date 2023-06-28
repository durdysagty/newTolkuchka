using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class CustomerGuidService : ServiceNoFile<CustomerGuid, CustomerGuid>, ICustomerGuid
    {
        public CustomerGuidService(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
        }

        public async Task AddModelAsync(CustomerGuid customerGuid)
        {
            await _con.CustomerGuids.AddAsync(customerGuid);
        }
    }
}

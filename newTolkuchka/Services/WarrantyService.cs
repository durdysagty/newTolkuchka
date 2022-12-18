using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class WarrantyService : ServiceNoFile<Warranty>, IWarranty
    {
        public WarrantyService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }

        public IEnumerable<AdminWarranty> GetAdminWarranties()
        {
            IEnumerable<AdminWarranty> warranties = GetModels().Select(x => new AdminWarranty
            {
                Id = x.Id,
                Name = x.NameRu
            }).OrderBy(x => x.Name);
            return warranties;
        }
    }
}
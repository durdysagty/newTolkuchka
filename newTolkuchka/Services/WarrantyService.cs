using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class WarrantyService : ServiceNoFile<Warranty>, IWarranty
    {
        public WarrantyService(AppDbContext con) : base(con)
        {
        }

        public IEnumerable<AdminWarranty> GetAdminWarranties()
        {
            IEnumerable<AdminWarranty> warranties = GetModels().Select(x => new AdminWarranty
            {
                Id = x.Id,
                Name = x.NameRu,
            }).OrderBy(x => x.Name);
            return warranties;
        }
    }
}
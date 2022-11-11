using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Type = newTolkuchka.Models.Type;

namespace newTolkuchka.Services
{
    public class TypeService : ServiceNoFile<Type>, IType
    {
        public TypeService(AppDbContext con) : base(con)
        {
        }

        public IEnumerable<AdminType> GetAdminTypes()
        {
            IEnumerable<AdminType> types = GetModels().Select(x => new AdminType
            {
                Id = x.Id,
                Name = x.NameRu,
                Products = x.Products.Count
            }).OrderBy(x => x.Name);
            return types;
        }
    }
}
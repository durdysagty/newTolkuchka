using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using Type = newTolkuchka.Models.Type;

namespace newTolkuchka.Services
{
    public class TypeService : ServiceNoFile<Type>, IType
    {
        public TypeService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }

        public IEnumerable<AdminType> GetAdminTypes()
        {
            IEnumerable<AdminType> types = GetModels().Select(x => new AdminType
            {
                Id = x.Id,
                Name = x.NameRu,
                Models = x.Models.Count
            }).OrderBy(x => x.Name);
            return types;
        }
    }
}
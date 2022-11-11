using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class SpecsValueService : ServiceFormFile<SpecsValue>, ISpecsValue
    {
        private const int IMAGESMAX = 1;
        public SpecsValueService(AppDbContext con, IPath path, IImage image) : base(con, path, image, IMAGESMAX)
        {
        }

        public IEnumerable<AdminSpecsValue> GetAdminSpecsValues(int specId)
        {
            IEnumerable<AdminSpecsValue> specsValues = GetModels().Where(x => x.SpecId == specId).Select(x => new AdminSpecsValue
            {
                Id = x.Id,
                Name = x.NameRu,
                Products = x.ProductSpecsValues.Count
            }).OrderBy(x => x.Name);
            return specsValues;
        }
    }
}
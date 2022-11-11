using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class LineService : ServiceNoFile<Line>, ILine
    {
        public LineService(AppDbContext con) : base(con)
        {
        }

        public IEnumerable<AdminLine> GetAdminLines(int? brandId)
        {
            IQueryable<Line> lines = GetModels();
            if (brandId != null)
                lines = lines.Where(l => l.BrandId == brandId);
            IEnumerable<AdminLine> adminLines = lines.Select(x => new AdminLine
            {
                Id = x.Id,
                Name = x.Name,
                Brand = x.Brand.Name,
                Products = x.Products.Count
            }).OrderBy(x => x.Name);
            return adminLines;
        }
    }
}

using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class LineService : ServiceNoFile<Line>, ILine
    {
        public LineService(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }

        public IEnumerable<AdminLine> GetAdminLines(int[] brandId)
        {
            IQueryable<Line> lines = GetModels();
            if (brandId.Any())
                lines = lines.Where(l => brandId.Any(b => b == l.BrandId));
            IEnumerable<AdminLine> adminLines = lines.Select(x => new AdminLine
            {
                Id = x.Id,
                Name = x.Name,
                Brand = x.Brand.Name,
                Models = x.Models.Count
            }).OrderBy(x => x.Name);
            return adminLines;
        }
    }
}

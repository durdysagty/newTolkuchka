using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IReport : IActionNoFile<Invoice, AdminReportOrder>
    {
        //Task<IList<AdminReoprtOrder>> CreatePeriodReport(DateTimeOffset start, DateTimeOffset end);
    }
}

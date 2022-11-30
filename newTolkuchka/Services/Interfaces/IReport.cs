using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IReport
    {
        Task<IList<AdminReoprtOrder>> CreatePeriodReport(DateTimeOffset start, DateTimeOffset end);
    }
}

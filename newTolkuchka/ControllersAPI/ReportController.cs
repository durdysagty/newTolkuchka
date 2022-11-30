using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [ApiController, Route("api/[controller]"), Authorize(Policy = "Level3")]
    public class ReportController : ControllerBase
    {
        private readonly IReport _report;
        public ReportController(IReport report)
        {
            _report = report;
        }

        [HttpGet]
        public async Task<IList<AdminReoprtOrder>> Get([FromQuery] string start, [FromQuery] string end)
        {
            return await _report.CreatePeriodReport(DateTimeOffset.Parse(start), DateTimeOffset.Parse(end));
        }
    }
}

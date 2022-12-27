using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [ApiController, Route("api/[controller]"), Authorize(Policy = "Level3")]
    public class ReportController : AbstractController<Invoice, AdminReoprtOrder, IReport>
    {
        //private readonly IReport _report;
        public ReportController(IEntry entry, IReport report) : base(entry, Entity.Default, report)
        {
            //_report = report;
        }

        //[HttpGet]
        //public async Task<IList<AdminReoprtOrder>> Get([FromQuery] string start, [FromQuery] string end)
        //{
        //    return await _report.CreatePeriodReport(DateTimeOffset.Parse(start), DateTimeOffset.Parse(end));
        //}
    }
}

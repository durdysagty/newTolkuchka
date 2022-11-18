using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PositionService : ServiceNoFile<Position>, IPosition
    {
        private readonly IEmployee _employee;
        public PositionService(AppDbContext con, IStringLocalizer<Shared> localizer, IEmployee employee) : base(con, localizer)
        {
            _employee = employee;
        }

        public IEnumerable<AdminPosition> GetAdminPositions()
        {
            IEnumerable<AdminPosition> positions = GetModels().Select(x => new AdminPosition
            {
                Id = x.Id,
                Name = x.Name,
                Level = x.Level,
                Employees = x.Employees.Count,
            }).OrderBy(x => x.Name);
            return positions;
        }

        public void EditPosition(Position position)
        {
            EditModel(position); // employees hash to be changed to make them reload with new credentials
            IQueryable<Employee> employees = _employee.GetEmployeesByPosition(position.Id);
            _employee.ChangeEmployeesHashAsync(employees);
        }
    }
}

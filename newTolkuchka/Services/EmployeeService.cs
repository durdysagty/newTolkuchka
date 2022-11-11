using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EmployeeService : ServiceNoFile<Employee>, IEmployee
    {
        private readonly ICrypto _crypto;
        public EmployeeService(AppDbContext con, ICrypto crypto) : base(con)
        {
            _crypto = crypto;
        }
        public async Task<EditEmployee> GetEditEmployeeAsync(int id)
        {
            EditEmployee employee = await GetModels().Where(x=>x.Id == id).Select(x => new EditEmployee
            {
                Id = x.Id,
                Login = x.Login,
                Password = "0",
                PositionId = x.PositionId
            }).FirstOrDefaultAsync();
            return employee;
        }

        public IEnumerable<AdminEmployee> GetAdminEmployees()
        {
            IEnumerable<AdminEmployee> employees = GetModels().Select(x => new AdminEmployee
            {
                Id = x.Id,
                Login = x.Login,
                Position = x.Position.Name,
                Level = x.Position.Level
            }).OrderBy(x => x.Id);
            return employees;
        }

        public override bool IsExist(Employee employee, IEnumerable<Employee> list)
        {
            bool exist = false;
            if (!list.Any())
                return exist;
                exist = list.Where(x => x.Login == employee.Login).Any();
            return exist;
        }

        public override async Task AddModelAsync(Employee employee, bool save)
        {
            EncryptPassword(employee);
            await _con.Employees.AddAsync(employee);
        }
        public async Task EditEmployeeAsync(EditEmployee editEmployee)
        {
            Employee employee = await GetModelAsync(editEmployee.Id);
            employee.PositionId = editEmployee.PositionId;
            ChangeEmployeesHashAsync(new[] { employee }); // employee hash to be changed to make the one reload with new credentials
            if (editEmployee.Password != "0")
            {
                employee.Password = editEmployee.Password;
                EncryptPassword(employee);
            }
            EditModel(employee);
        }

        public async Task<Employee> GetEmployeeWithPositionAsync(string login)
        {
            Employee employee = await GetModels().Where(x => x.Login == login).Include(x => x.Position).FirstOrDefaultAsync();
            return employee;
        }

        public IQueryable<Employee> GetEmployeesByPosition(int id)
        {
            IQueryable<Employee> employees = GetModels().Where(x => x.PositionId == id);
            return employees;
        }

        public void ChangeEmployeesHashAsync(IEnumerable<Employee> employees)
        {
            foreach (Employee employee in employees)
                employee.Hash = ICrypto.GetNumber(0, 1000).ToString(); // to be more complicated hash
        }

        private void EncryptPassword(Employee employee) => employee.Password = _crypto.EncryptString(employee.Password);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class EmployeeController : AbstractController<Employee, AdminEmployee, IEmployee>
    {
        private readonly IEmployee _employee;
        public EmployeeController(IEntry entry, IEmployee employee) : base(entry, Entity.Employee, employee)
        {
            _employee = employee;
        }

        [HttpGet("{id}")]
        public async Task<EditEmployee> Get(int id)
        {
            EditEmployee employee = await _employee.GetEditEmployeeAsync(id);
            return employee;
        }
        //[HttpGet]
        //public IEnumerable<AdminEmployee> Get()
        //{
        //    IEnumerable<AdminEmployee> employees = _employee.GetAdminEmployees();
        //    return employees;
        //}
        [HttpPost]
        public async Task<Result> Post(Employee employee)
        {
            bool isExist = _employee.IsExist(employee, _employee.GetModels());
            if (isExist)
                return Result.Already;
            await _employee.AddModelAsync(employee);
            await AddActAsync(employee.Id, employee.Login);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(EditEmployee employee)
        {
            await _employee.EditEmployeeAsync(employee);            
            await EditActAsync(employee.Id, employee.Login);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Employee employee = await _employee.GetModelAsync(id);
            if (employee == null)
                return Result.Fail;
            _employee.DeleteHash(id);
            Result result = await _employee.DeleteModelAsync(employee.Id, employee);
            if (result == Result.Success)
                await DeleteActAsync(id, employee.Login);
            return result;
        }
    }
}
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
        public EmployeeController(IEntry entry, IEmployee employee, ICacheClean cacheClean) : base(entry, Entity.Employee, employee, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<EditEmployee> Get(int id)
        {
            EditEmployee employee = await _service.GetEditEmployeeAsync(id);
            return employee;
        }
        //[HttpGet]
        //public IEnumerable<AdminEmployee> Get()
        //{
        //    IEnumerable<AdminEmployee> employees = _service.GetAdminEmployees();
        //    return employees;
        //}
        [HttpPost]
        public async Task<Result> Post(Employee employee)
        {
            bool isExist = _service.IsExist(employee, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(employee);
            await AddActAsync(employee.Id, employee.Login);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(EditEmployee employee)
        {
            await _service.EditEmployeeAsync(employee);            
            await EditActAsync(employee.Id, employee.Login);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Employee employee = await _service.GetModelAsync(id);
            if (employee == null)
                return Result.Fail;
            _service.DeleteHash(id);
            Result result = await _service.DeleteModelAsync(employee.Id, employee);
            if (result == Result.Success)
                await DeleteActAsync(id, employee.Login);
            return result;
        }
    }
}
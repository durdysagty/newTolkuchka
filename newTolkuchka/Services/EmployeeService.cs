﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class EmployeeService : ServiceNoFile<Employee, AdminEmployee>, IEmployee
    {
        private readonly ICrypto _crypto;
        public EmployeeService(AppDbContext con, IMemoryCache memoryCache, IStringLocalizer<Shared> localizer, ICacheClean cacheClean, ICrypto crypto) : base(con, memoryCache, localizer, cacheClean)
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

        public override bool IsExist(Employee employee, IEnumerable<Employee> list)
        {
            bool exist = false;
            if (!list.Any())
                return exist;
                exist = list.Where(x => x.Login == employee.Login).Any();
            return exist;
        }

        public override async Task AddModelAsync(Employee employee, bool save = true)
        {
            EncryptPassword(employee);
            employee.Hash = "1";
            await _con.Employees.AddAsync(employee);
            if (save)
                _ = await _con.SaveChangesAsync();
            //_cacheClean.CleanAdminModels(typeof(Employee));
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
            //_cacheClean.CleanAdminModels(typeof(Employee));
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
            {
                employee.Hash = ICrypto.GetNumber(0, 1000).ToString(); // to be more complicated hash
                DeleteHash(employee.Id);
            }
        }

        public void DeleteHash(int id)
        {
            _memoryCache.Remove(ConstantsService.EmpHashKey(id));
        }

        private void EncryptPassword(Employee employee) => employee.Password = _crypto.EncryptString(employee.Password);
    }
}

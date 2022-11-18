using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEmployee: IActionNoFile<Employee>
    {
        Task<EditEmployee> GetEditEmployeeAsync(int id);
        IEnumerable<AdminEmployee> GetAdminEmployees();
        IQueryable<string> GetEmployeeNames(int[] ids);
        Task EditEmployeeAsync(EditEmployee editEmployee);
        Task<Employee> GetEmployeeWithPositionAsync(string login);
        IQueryable<Employee> GetEmployeesByPosition(int id);
        void ChangeEmployeesHashAsync(IEnumerable<Employee> employees);
    }
}

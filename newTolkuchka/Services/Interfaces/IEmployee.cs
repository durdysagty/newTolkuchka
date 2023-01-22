using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IEmployee: IActionNoFile<Employee, AdminEmployee>
    {
        Task<EditEmployee> GetEditEmployeeAsync(int id);
        //IQueryable<string> GetEmployeeNames(int[] ids);
        Task EditEmployeeAsync(EditEmployee editEmployee);
        Task<Employee> GetEmployeeWithPositionAsync(string login);
        IQueryable<Employee> GetEmployeesByPosition(int id);
        void ChangeEmployeesHashAsync(IEnumerable<Employee> employees);
        void DeleteHash(int id);
    }
}

using newTolkuchka.Models;

namespace newTolkuchka.Services.Interfaces
{
    public interface IJwt
    {
        const string jwtKey = Secrets.jwtKey;
        string GetEmployeeToken(Employee employee);
        string GetUserToken(User user);
        string GetFalseToken();
    }
}

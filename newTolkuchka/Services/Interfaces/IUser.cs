using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IUser : IActionNoFile<User, AdminUser>
    {
        Task<User> GetCurrentUser();
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByIdAsync(int id);
        Task<AdminUserData> GetAdminUserDataAsync(int id);
        // returns is pin changed status
        Task<bool> EditUserAsync(AccountUser accountUser, User user);
    }
}

using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IUser : IActionNoFile<User>
    {
        Task<User> GetCurrentUser();
        Task<User> GetUserByLoginAsync(string login);
        // returns is pin changed status
        Task<bool> EditUserAsync(AccountUser accountUser, User user);
    }
}

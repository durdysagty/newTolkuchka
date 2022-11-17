using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace newTolkuchka.Services
{
    public class UserService : ServiceNoFile<User>, IUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICrypto _crypto;
        public UserService(AppDbContext con, IHttpContextAccessor contextAccessor, ICrypto crypto) : base(con)
        {
            _contextAccessor = contextAccessor;
            _crypto = crypto;
        }

        public async Task<User> GetCurrentUser()
        {
            string userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _contextAccessor.HttpContext.Response.Cookies.Delete(Secrets.userCookie);
                return null;

            }
            User user = await GetModelAsync(int.Parse(userId));
            return user;
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            User user = await GetModels().FirstOrDefaultAsync(u => u.Email == login);
            return user;
        }

        public async Task<bool> EditUserAsync(AccountUser accountUser, User user)
        {
            if (accountUser.Pin is 0 or (< 9999 and > 999))
            {
                user.Name = accountUser.Name;
                user.Phone = accountUser.Phone;
                user.Address = accountUser.Address;
                user.BirthDay = accountUser.BirthDay;
                user.Pin = _crypto.EncryptString(accountUser.Pin.ToString());
                EditModel(user);
                await SaveChangesAsync();
            }
            if (accountUser.Pin is 0)
                return false;
            else
                return true;
        }
    }
}

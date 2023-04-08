using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class UserController : AbstractController<User, AdminUser, IUser>
    {
        private readonly IUser _user;
        public UserController(IEntry entry, IUser user) : base(entry, Entity.User, user)
        {
            _user = user;
        }

        [HttpGet("{id}")]
        public async Task<AdminUserData> Get(int id)
        {
            AdminUserData user = await _user.GetAdminUserDataAsync(id);
            return user;
        }
    }
}
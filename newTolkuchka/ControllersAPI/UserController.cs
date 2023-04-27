using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class UserController : AbstractController<User, AdminUser, IUser>
    {
        public UserController(IEntry entry, IUser user, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.User, user, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<AdminUserData> Get(int id)
        {
            AdminUserData user = await _service.GetAdminUserDataAsync(id);
            return user;
        }
    }
}
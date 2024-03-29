﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Security.Claims;

namespace newTolkuchka.Services
{
    public class UserService : ServiceNoFile<User, AdminUser>, IUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICrypto _crypto;
        public UserService(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean, IHttpContextAccessor contextAccessor, ICrypto crypto) : base(con, localizer, cacheClean)
        {
            _contextAccessor = contextAccessor;
            _crypto = crypto;
        }

        public async Task<User> GetCurrentUser()
        {
            string userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _contextAccessor.HttpContext.Response.Cookies.Delete(Secrets.userHashCookie);
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

        public async Task<User> GetUserByIdAsync(int id)
        {
            User user = await GetModelAsync(id);
            return user;
        }

        public async Task<AdminUserData> GetAdminUserDataAsync(int id)
        {
            AdminUserData adminUserData = await GetModels().Where(x => x.Id == id).Select(x => new AdminUserData
            {
                Id = x.Id,
                Email = x.Email,
                HumanName = x.Name,
                Phone = x.Phone,
                Address = x.Address,
                BirthDay = x.BirthDay,
                Invoices = x.Invoices.Count,
                Wishes = x.Wishes.Count
            }).FirstOrDefaultAsync();
            return adminUserData;
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

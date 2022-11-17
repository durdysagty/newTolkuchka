using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Text.RegularExpressions;
using static newTolkuchka.Models.DTO.LoginResponse;

namespace newTolkuchka.Services
{
    public class LoginService : ILogin
    {
        private readonly AppDbContext _con;
        private readonly ICrypto _crypto;
        private readonly IJwt _jwt;
        private readonly IEmployee _employee;
        private readonly IUser _user;
        private readonly IMail _mail;
        private readonly IStringLocalizer<Shared> _localizer;
        private readonly IMemoryCache _memoryCache;
        public LoginService(AppDbContext con, IJwt jwt, ICrypto crypto, IEmployee employee, IUser user, IMail mail, IStringLocalizer<Shared> localizer, IMemoryCache memoryCache)
        {
            _con = con;
            _crypto = crypto;
            _jwt = jwt;
            _employee = employee;
            _user = user;
            _mail = mail;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }
        public async Task<LoginResponse> LoginFirstStepByEmailAsync(string login)
        {
            bool isEmail = Regex.IsMatch(login, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            if (!isEmail)
                return CreateFailResult(_localizer["wrong-email"]);
            User user = await _user.GetUserByLoginAsync(login);
            if (user == null)
            {
                int pin = ICrypto.GetNumber(1000, 9999);
                bool isSent = await _mail.SendPinAsync(login, pin);
                if (!isSent)
                    return CreateFailResult(_localizer["no-sent-email"]);
                user = new()
                {
                    Email = login,
                    Pin = _crypto.EncryptString(pin.ToString())
                };
                await _user.AddModelAsync(user);
                _memoryCache.Set(user.Id, pin, TimeSpan.FromMinutes(5));
                return new LoginResponse
                {
                    Result = R.New,
                    Data = user.Id.ToString(),
                    Text = _localizer["new-registered"]
                };
            }
            int userPin = int.Parse(_crypto.DecryptString(user.Pin));
            _memoryCache.Set(user.Id, userPin, TimeSpan.FromMinutes(5));
            return new LoginResponse
            {
                Result = R.Success,
                Data = user.Id.ToString()
            };
        }
        public async Task<LoginResponse> LoginSecondStepAsync(string pinNumbers)
        {
            int id = (int)char.GetNumericValue(pinNumbers[0]);
            int p = int.Parse(pinNumbers[1..]);
            _memoryCache.TryGetValue(id, out int pin);
            if (pin != p)
                return CreateFailResult(_localizer["wrong-pin"]);
            User user = await _con.Users.FindAsync(id);
            string token = _jwt.GetUserToken(user);
            return new LoginResponse
            {
                Result = R.Success,
                Data = token
            };
        }


        public async Task<LoginResponse> JwtLoginEmployeeAsync(LoginRequest loginRequest)
        {
            Employee employee = await _employee.GetEmployeeWithPositionAsync(loginRequest.Login);
            if (employee == null)
                return CreateFailResult();
            if (employee.Password != _crypto.EncryptString(loginRequest.Password))
                return CreateFailResult();
            return CreateEmployeeSuccessResult(employee);
        }
        public async Task<LoginResponse> CheckAuthedEmployeeAsync(HttpContext httpContext)
        {
            string id = IContext.GetAthorizedUserId(httpContext);
            Employee employee = await _employee.GetEmployeeWithPositionAsync(id);
            if (employee == null)
                return CreateFailResult();
            string hash = IContext.GetAthorizedUserHash(httpContext);
            if (employee.Hash != hash)
                return CreateFailResult();
            return CreateEmployeeSuccessResult(employee);
        }


        private LoginResponse CreateEmployeeSuccessResult(Employee employee)
        {
            string token = _jwt.GetEmployeeToken(employee);
            return new LoginResponse { Result = R.Success, Data = token, Text = Secrets.adminCookie };
        }
        private static LoginResponse CreateFailResult(string text = null) => new() { Result = R.Fail, Text = text };
    }
}

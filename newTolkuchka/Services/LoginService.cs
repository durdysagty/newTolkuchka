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
    public partial class LoginService : ILogin
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
            bool isEmail = MyRegex().IsMatch(login);
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
                _memoryCache.Set(ConstantsService.UserPinKey(user.Id), pin, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.NeverRemove,
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
                return new LoginResponse
                {
                    Result = R.New,
                    Data = user.Id.ToString(),
                    Text = _localizer["new-registered"]
                };
            }
            int userPin = int.Parse(_crypto.DecryptString(user.Pin));
            _memoryCache.Set(ConstantsService.UserPinKey(user.Id), userPin, TimeSpan.FromMinutes(5));
            return new LoginResponse
            {
                Result = R.Success,
                Data = user.Id.ToString()
            };
        }
        public async Task<LoginResponse> LoginSecondStepAsync(string pinNumbers)
        {
            GetUserIdPin(pinNumbers, out int id, out int p);
            string key = ConstantsService.PIN + ConstantsService.USER + id;
            _memoryCache.TryGetValue(key, out int? count);
            if (count == 3)
                return CreateFailResult(_localizer["attempts"]);
            _memoryCache.TryGetValue(ConstantsService.USER + id, out int? pin);
            if (pin == null)
                return CreateFailResult(_localizer["time-elapsed1"]);
            if (pin != p)
            {
                if (count == null)
                    count = 1;
                else count++;
                _memoryCache.Set(ConstantsService.PIN + ConstantsService.USER + id, count, TimeSpan.FromHours(3));
                if (count == 3)
                    _memoryCache.Remove(ConstantsService.USER + id);
                return CreateFailResult(_localizer["wrong-pin"]);
            }
            User user = await _con.Users.FindAsync(id);
            string hash = _crypto.EncryptString($"{user.Id} {user.Pin}{user.Email}");
            string token = _jwt.GetUserToken(user);
            // could be used for count actual loged in users in the future
            _ = _memoryCache.Set(ConstantsService.UserHashKey(user.Id), hash, TimeSpan.FromDays(28));
            return new LoginResponse
            {
                Result = R.Success,
                Data = token,
                Text = hash
            };
        }

        public async Task<LoginResponse> RecoveryAsync(string pinNumbers)
        {
            GetUserIdPin(pinNumbers, out int id, out int _);
            User user = await _user.GetUserByIdAsync(id);
            if (user == null)
                return CreateFailResult(_localizer["no-user"]);
            Guid guid = Guid.NewGuid();
            bool isSent = await _mail.SendRecoveryAsync(user.Email, guid);
            if (!isSent)
                return CreateFailResult(_localizer["no-sent-email"]);
            _memoryCache.Set(guid, user.Id, TimeSpan.FromDays(1));
            return new LoginResponse
            {
                Result = R.Success,
                Text = _localizer["new-registered"]
            };
        }
        public async Task<LoginResponse> NewPINAsync(Guid guid)
        {
            _memoryCache.TryGetValue(guid, out int? id);
            if (id == null)
            {
                _memoryCache.TryGetValue(guid + "used", out int? count);
                if (count != null)
                {
                    if (count < 3)
                    {
                        _memoryCache.Set(guid + "used", ++count, TimeSpan.FromHours(12));
                        return CreateFailResult(_localizer["time-elapsed"]);
                    }
                    else
                    {
                        _memoryCache.Remove(guid + "used");
                        return new() { Result = R.Error };
                    }
                }
                else
                    return new() { Result = R.Error };
            }
            User user = await _user.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                _memoryCache.Remove(guid);
                return CreateFailResult(_localizer["no-user"]);
            }
            int pin = ICrypto.GetNumber(1000, 9999);
            bool isSent = await _mail.SendNewPinAsync(user.Email, pin);
            if (!isSent)
                return CreateFailResult(_localizer["no-sent-email1"]);
            user.Pin = _crypto.EncryptString(pin.ToString());
            await _user.SaveChangesAsync();
            _memoryCache.Remove(guid);
            _memoryCache.Set(guid + "used", 1, TimeSpan.FromHours(12));
            return new LoginResponse
            {
                Result = R.Success,
                Text = _localizer["new-pin"].Value
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

        private static void GetUserIdPin(string pinNumbers, out int id, out int p)
        {
            id = int.Parse(pinNumbers.Remove(pinNumbers.Length - 4));
            p = int.Parse(pinNumbers[^4..]);

        }

        private LoginResponse CreateEmployeeSuccessResult(Employee employee)
        {
            string token = _jwt.GetEmployeeToken(employee);
            _ = _memoryCache.Set(ConstantsService.EmpHashKey(employee.Id), $"{employee.Id}-{employee.Hash}", TimeSpan.FromDays(7));
            // Text is employeeId and employee Hash we use '-' to split them on Program if there are space then cokkie are not sending
            return new LoginResponse { Result = R.Success, Data = token, Text = $"{employee.Id}-{employee.Hash}" };
        }
        private static LoginResponse CreateFailResult(string text = null) => new() { Result = R.Fail, Text = text };

        [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.IgnoreCase, "ru-RU")]
        private static partial Regex MyRegex();
    }
}

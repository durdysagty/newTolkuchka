using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _login;
        public LoginController(ILogin login)
        {
            _login = login;
        }

        [HttpPost("userlogin")]
        public async Task<LoginResponse> UserLogin([FromBody] string login)
        {
            LoginResponse loginResponse = await _login.LoginFirstStepByEmailAsync(login);
            return loginResponse;
        }

        [HttpPost("userpin")]
        public async Task<LoginResponse> UserSecondLogin([FromBody] string pins)
        {
            LoginResponse loginResponse = await _login.LoginSecondStepAsync(pins);
            if (loginResponse.Result == LoginResponse.R.Success)
            {
                HttpContext.Response.Cookies.Append(ConstantsService.USERCOOKIE, loginResponse.Data, new CookieOptions { MaxAge = new TimeSpan(10, 0, 0, 0) });
            }
            return loginResponse;
        }

        [HttpPost]
        public async Task<LoginResponse> Login(LoginRequest loginData)
        {
            LoginResponse loginResponse = await _login.JwtLoginEmployeeAsync(loginData);
            return loginResponse;
        }

        [HttpGet]
        public async Task<LoginResponse> CheckEmployee()
        {
            LoginResponse loginResponse = await _login.CheckAuthedEmployeeAsync(HttpContext);
            return loginResponse;
        }
    }
}

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
                HttpContext.Response.Cookies.Append(Secrets.userTokenCookie, loginResponse.Data, new CookieOptions
                {
                    MaxAge = new TimeSpan(29, 0, 0, 0),
                    // remove on publish
                    //SameSite = SameSiteMode.Strict,
                    //Domain = CultureProvider.Host,
                    //Secure = true
                });
                HttpContext.Response.Cookies.Append(Secrets.userHashCookie, loginResponse.Text, new CookieOptions
                {
                    MaxAge = new TimeSpan(28, 0, 0, 0),
                    // remove on publish
                    //SameSite = SameSiteMode.Strict,
                    //Domain = CultureProvider.Host,
                    //Secure = true
                });
                HttpContext.Response.Cookies.Append(Secrets.userFalseCookie, loginResponse.False, new CookieOptions
                {
                    MaxAge = new TimeSpan(28, 0, 0, 0),
                    // remove on publish
                    //SameSite = SameSiteMode.Strict,
                    //Domain = CultureProvider.Host,
                    //Secure = true
                });
            }
            return loginResponse;
        }

        //[HttpGet("logout")]
        //public void Logout()
        //{
        //    HttpContext.Response.Cookies.Delete(Secrets.userHashCookie);
        //    HttpContext.Response.Cookies.Delete(Secrets.userTokenCookie);
        //}

        [HttpPost("recovery")]
        public async Task<LoginResponse> UserRecoveryLogin([FromBody] string pins)
        {
            LoginResponse loginResponse = await _login.RecoveryAsync(pins);
            return loginResponse;
        }


        [HttpPost]
        public async Task<LoginResponse> Login(LoginRequest loginData)
        {
            LoginResponse loginResponse = await _login.JwtLoginEmployeeAsync(loginData);
            return loginResponse;
        }
    }
}

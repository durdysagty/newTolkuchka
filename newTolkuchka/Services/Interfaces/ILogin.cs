using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ILogin
    {
        Task<LoginResponse> LoginFirstStepByEmailAsync(string login);
        Task<LoginResponse> LoginSecondStepAsync(string pinNumbers);


        Task<LoginResponse> JwtLoginEmployeeAsync(LoginRequest loginRequest);
        Task<LoginResponse> CheckAuthedEmployeeAsync(HttpContext httpContext);
    }
}

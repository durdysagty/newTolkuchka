using System.Security.Claims;

namespace newTolkuchka.Services.Interfaces
{
    public interface IContext
    {
        public static string GetAthorizedUserId(HttpContext httpContext) => httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        public static string GetAthorizedUserHash(HttpContext httpContext) => httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Hash).Value;
    }
}

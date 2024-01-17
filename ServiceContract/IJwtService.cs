using OrdersWebAPI.EfCore;
using OrdersWebAPI.Response;
using System.Security.Claims;

namespace OrdersWebAPI.ServiceContract
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}

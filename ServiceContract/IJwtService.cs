using OrdersWebAPI.EfCore;
using OrdersWebAPI.Response;

namespace OrdersWebAPI.ServiceContract
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);
    }
}

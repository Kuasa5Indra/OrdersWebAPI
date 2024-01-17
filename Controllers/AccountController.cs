using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrdersWebAPI.EfCore;
using OrdersWebAPI.Request;
using OrdersWebAPI.Response;
using OrdersWebAPI.ServiceContract;
using System.Security.Claims;

namespace OrdersWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterAccountRequest request)
        {
            ApplicationUser user = new()
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName,
                PersonName = request.PersonName
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password); 

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                var authenticationResponse = _jwtService.CreateJwtToken(user);
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;

                await _userManager.UpdateAsync(user);
                return Ok(authenticationResponse);
            }
            else
            {
                string errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    return NoContent();
                }

                var authenticationResponse = _jwtService.CreateJwtToken(user);

                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;

                await _userManager.UpdateAsync(user);

                return Ok(authenticationResponse);
            }
            else 
            {
                return Problem("Invalid username or password");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateNewToken(TokenRequest request)
        {
            if(request == null)
            {
                return BadRequest("Invalid client request");
            }

            string? token = request.Token;
            string? refreshToken = request.RefreshToken;

            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(token);

            if (principal == null)
            {
                return BadRequest("Invalid jwt access token");
            }

            string? id = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser? user = await _userManager.FindByIdAsync(id);

            if(user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return BadRequest("Invalid Refresh Token");
            }

            AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);

            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return Ok(authenticationResponse);
        }
    }
}

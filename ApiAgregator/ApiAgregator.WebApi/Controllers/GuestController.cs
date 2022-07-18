using ApiAgregator.Services;
using ApiAgregator.Services.Exceptions;
using ApiAgregator.WebApi.Models.Request;
using ApiAgregator.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ApiAgregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GuestController : ControllerBase
    {
        [HttpPost]
        public IActionResult SignUp([FromBody] SignUpRequest request, [FromServices] AuthenticationService auth)
        {
            try
            {
                auth.Register(request.Username, request.Email, request.Password);
            }
            catch (UsernameExistsException)
            {
                return Conflict("username already in use");
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult SignInByUsername([FromBody] SignInByUsernameRequest request, 
            [FromServices] AuthenticationService auth, [FromServices] JwtService authHandler)
        {
            try
            {
                var user = auth.LogInByUsername(request.Username, request.Password);
                return Ok(new
                {
                    jwt = authHandler.CreateToken(user),
                    login = user.Username,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed,
                    isAdmin = user.IsAdmin,
                });
            }
            catch (UserDoesNotExistException)
            {
                return BadRequest("username or password do not match");
            }
        }
    }
}

using ApiAgregator.Services;
using ApiAgregator.Services.Exceptions;
using ApiAgregator.Services.Repositories;
using ApiAgregator.WebApi.Models.Request;
using ApiAgregator.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAgregator.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmailValidationController : ControllerBase
    {
        [HttpGet]
        public IActionResult RequestConfirmation(
            [FromServices] EmailValidationService emailValidation)
        {
            try
            {
                emailValidation.RequestEmailValidation(ClaimsParser.GetUserId(User));
            }
            catch (EmailAlreadyConfirmedException)
            {
                return Conflict("email already confirmed");
            }
            
            return Ok();
        }

        [HttpPost]
        public IActionResult Confirm([FromBody] ConfirmRequest request, [FromServices] EmailValidationService emailValidation)
        {
            try
            {
                emailValidation.ValidateEmail(request.Token);
            }
            catch (EmailValidationTokenException)
            {
                return Forbid();
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult CheckConfirmation([FromServices] IUserRepository userRepository,
            [FromServices] JwtService authHandler)
        {
            var user = userRepository.GetById(ClaimsParser.GetUserId(User));
            if (user.EmailConfirmed)
            {
                return Ok(new
                {
                    jwt = authHandler.CreateToken(user),
                    login = user.Username,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed,
                    isAdmin = user.IsAdmin,
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}

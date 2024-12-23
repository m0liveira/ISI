using Microsoft.AspNetCore.Mvc;
using futFind.Services;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _AuthService;

        public AuthController(AuthService authService)
        {
            _AuthService = authService;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] LoginRequest login)
        {

            var token = _AuthService.GenerateToken(login.Password, login.Email);

            if (token == null) { return Unauthorized(new { message = "Invalid username or password" }); }

            return Ok(new { token });
        }
    }
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
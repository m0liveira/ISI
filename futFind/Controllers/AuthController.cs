using Microsoft.AspNetCore.Mvc;
using futFind.Services;
using futFind.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using futFind.Swagger.Requests;
using futFind.Swagger.Responses;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _AuthService;
        private readonly AppDbContext _context;

        public AuthController(AuthService authService, AppDbContext context)
        {
            _AuthService = authService;
            _context = context;

        }

        /// <summary> Authenticates a user using email and password. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
        [SwaggerOperation(Summary = "User authentication", Description = "Authenticates a user and generates a JWT token if the email and password are valid.")]
        [SwaggerRequestExample(typeof(LoginRequest), typeof(AuthRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthorizedResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
        [HttpPost]  // POST: /api/Auth
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest login)
        {
            var user = await _context.users.FirstOrDefaultAsync(res => res.email == login.Email);

            if (user == null || user.password != login.Password)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _AuthService.GenerateToken(user.id.ToString(), user.email);

            var response = new { token, data = user };

            return Ok(response);
        }
    }
}
public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
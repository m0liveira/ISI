using Microsoft.AspNetCore.Mvc;
using futFind.Services;
using futFind.Models;
using Microsoft.EntityFrameworkCore;

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



        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest login)
        {
            var user = await _context.users.FirstOrDefaultAsync(res => res.email == login.Email);

            if (user == null || user.password != login.Password) { return Unauthorized(new { message = "Invalid email or password" }); }

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
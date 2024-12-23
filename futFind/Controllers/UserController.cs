using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        private bool UserExists(int id) { return _context.users.Any(res => res.id == id); }

        private bool EmailExists(string email) { return _context.users.Any(res => res.email == email); }

        private bool PhoneExists(string phone) { return _context.users.Any(res => res.phone == phone); }

        // GET: /api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.users.ToListAsync());
        }

        // GET: /api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Users>>> GetUser(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!UserExists(id)) { return BadRequest(NotFound()); }

            return Ok(await _context.users.FindAsync(id));
        }

        // POST: /api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Users>>> CreateUser(Users user)
        {
            if (EmailExists(user.email)) { return Conflict(new { message = "Email is already in use." }); }

            if (PhoneExists(user.phone)) { return Conflict(new { message = "Phone is already in use." }); }

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        // PUT: /api/Users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<Users>>> UpdateUser(int id, Users user)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!UserExists(id)) { return NotFound(new { message = "User not found." }); }

            if (EmailExists(user.email)) { return Conflict(new { message = "Email is already in use." }); }

            if (PhoneExists(user.phone)) { return Conflict(new { message = "Phone number is already in use." }); }

            var existingUser = await _context.users.FindAsync(id);

            if (existingUser == null) { return NotFound(new { message = "User not found." }); }

            existingUser.name = user.name;
            existingUser.email = user.email;
            existingUser.password = user.password;
            existingUser.phone = user.phone;

            _context.Entry(existingUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }


        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<Users>>> DeleteUser(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!UserExists(id)) { return NotFound(new { message = "User not found." }); }

            var user = await _context.users.FindAsync(id);
            if (user == null) { return NotFound(new { message = "User not found." }); }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }
    }
}

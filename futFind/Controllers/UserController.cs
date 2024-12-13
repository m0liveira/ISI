using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        private bool UserExists(int id)
        {
            return _context.users.Any(res => res.id == id);
        }

        // GET: /api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Users>>> GetUser(int id)
        {
            if (!UserExists(id)) { return Ok(NotFound()); }

            return Ok(await _context.users.FindAsync(id));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using futFind.Models;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        private bool GameExists(int id) { return _context.games.Any(g => g.id == id); }

        // GET: /api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Games>>> GetGames() { return Ok(await _context.games.ToListAsync()); }

        // GET: /api/Games/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Games>> GetGame(int id)
        {
            if (!GameExists(id)) { return NotFound(new { message = "Game not found." }); }

            return Ok(await _context.games.FindAsync(id));
        }

        // GET: /api/Games/{share_code}
        [HttpGet("/Code/{share_code}")]
        public async Task<ActionResult<Games>> GetGamebyCode(string share_code)
        {
            var game = await _context.games.FirstOrDefaultAsync(res => res.share_code == share_code);

            if (game == null) { return NotFound(new { message = "Game with the provided invite code was not found." }); }

            return Ok(game);
        }

        // POST: /api/Games
        [HttpPost]
        public async Task<ActionResult<Games>> CreateGame(Games game)
        {
            _context.games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.id }, game);
        }

        // PUT: /api/Games/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Games>> UpdateGame(int id, Games game)
        {
            if (!GameExists(id)) { return NotFound(new { message = "Game not found." }); }

            var existingGame = await _context.games.FindAsync(id);

            if (existingGame == null) { return NotFound(new { message = "Game not found." }); }

            existingGame.host_id = game.host_id;
            existingGame.date = game.date;
            existingGame.address = game.address;
            existingGame.capacity = game.capacity;
            existingGame.price = game.price;
            existingGame.is_private = game.is_private;
            existingGame.share_code = game.share_code;
            existingGame.status = game.status;

            _context.Entry(existingGame).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(existingGame);
        }

        // DELETE: /api/Games/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Games>> DeleteGame(int id)
        {
            if (!GameExists(id)) { return NotFound(new { message = "Game not found." }); }

            var game = await _context.games.FindAsync(id);
            if (game == null) { return NotFound(new { message = "Game not found." }); }

            _context.games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Game deleted successfully." });
        }
    }
}

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
    public class PlayerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        private bool PlayerExists(int userId, int matchId)
        {
            return _context.players.Any(p => p.user_id == userId && p.match_id == matchId);
        }

        // GET: /api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Players>>> GetPlayers()
        {
            return Ok(await _context.players.ToListAsync());
        }

        // GET: /api/Players/{userId}/{matchId}
        [HttpGet("{userId}/{matchId}")]
        public async Task<ActionResult<Players>> GetPlayer(int userId, int matchId)
        {
            if (!PlayerExists(userId, matchId)) { return NotFound(new { message = "Player not found." }); }

            return Ok(await _context.players
                .FirstOrDefaultAsync(p => p.user_id == userId && p.match_id == matchId));
        }

        // POST: /api/Players
        [HttpPost]
        public async Task<ActionResult<Players>> AddPlayer(Players player)
        {
            if (PlayerExists(player.user_id, player.match_id))
            {
                return Conflict(new { message = "Player is already registered for this match." });
            }

            _context.players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { userId = player.user_id, matchId = player.match_id }, player);
        }

        // DELETE: /api/Players/{userId}/{matchId}
        [HttpDelete("{userId}/{matchId}")]
        public async Task<ActionResult> RemovePlayer(int userId, int matchId)
        {
            if (!PlayerExists(userId, matchId)) { return NotFound(new { message = "Player not found." }); }

            var player = await _context.players
                .FirstOrDefaultAsync(p => p.user_id == userId && p.match_id == matchId);

            if (player == null) { return NotFound(new { message = "Player not found." }); }

            _context.players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Player removed successfully." });
        }
    }
}

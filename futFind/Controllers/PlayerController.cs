using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.players.ToListAsync());
        }

        [HttpGet("{userId}/{matchId}")]
        public async Task<ActionResult<Players>> GetPlayer(int userId, int matchId)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!PlayerExists(userId, matchId)) { return NotFound(new { message = "Player not found." }); }

            return Ok(await _context.players.FirstOrDefaultAsync(p => p.user_id == userId && p.match_id == matchId));
        }

        // GET: /api/Players/Game/{game_id}
        [HttpGet("Game/{game_id}")]
        public async Task<ActionResult<IEnumerable<Users>>> GetGamePlayers(int game_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var gameExists = await _context.games.AnyAsync(res => res.id == game_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            var players = await _context.players.Where(res => res.match_id == game_id).Select(res => res.User).ToListAsync();

            if (!players.Any()) { return NotFound(new { message = "No players found in the match." }); }

            return Ok(players);
        }

        // GET: /api/Players/{user_id}/Games
        [HttpGet("{user_id}/Games")]
        public async Task<ActionResult<IEnumerable<Users>>> GetPlayerGames(int user_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var userExists = await _context.users.AnyAsync(res => res.id == user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var games = await _context.players.Where(res => res.user_id == user_id).Select(res => res.Game).ToListAsync();

            if (!games.Any()) { return NotFound(new { message = "The player is not in any games." }); }

            return Ok(games);
        }

        // POST: /api/Players
        [HttpPost]
        public async Task<ActionResult<Players>> AddPlayer(Players player)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var userExists = await _context.users.AnyAsync(u => u.id == player.user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var gameExists = await _context.games.AnyAsync(t => t.id == player.match_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            var playerExists = await _context.players.AnyAsync(cm => cm.user_id == player.user_id && cm.match_id == player.match_id);
            if (playerExists) { return Conflict(new { message = "User is already a member of the match." }); }

            var newPlayer = new Players
            {
                user_id = player.user_id,
                match_id = player.match_id
            };

            _context.players.Add(newPlayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { userId = newPlayer.user_id, matchId = newPlayer.match_id }, newPlayer);
        }

        // DELETE: /api/Players/{userId}/{matchId}
        [HttpDelete("{userId}/{matchId}")]
        public async Task<ActionResult> RemovePlayer(int userId, int matchId)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using futFind.Swagger.Shared;

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

        /// <summary> Retrieves all players. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Players>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get a list of players",
            Description = "Fetches a list of teams. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpGet]   // GET: /api/Players
        public async Task<ActionResult<IEnumerable<Players>>> GetPlayers()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.players.ToListAsync());
        }

        /// <summary> Retrieves a specific players of a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Players))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a player of a game",
            Description = "Fetches a user data that is in a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{userId}/{matchId}")]
        public async Task<ActionResult<Players>> GetPlayer(int userId, int matchId)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!PlayerExists(userId, matchId)) { return NotFound(new { message = "Player not found." }); }

            return Ok(await _context.players.FirstOrDefaultAsync(p => p.user_id == userId && p.match_id == matchId));
        }

        /// <summary> Retrieves all players of a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Players>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get game players",
            Description = "Fetches all players of a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("Game/{game_id}")]    // GET: /api/Players/Game/{game_id}
        public async Task<ActionResult<IEnumerable<Users>>> GetGamePlayers(int game_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var gameExists = await _context.games.AnyAsync(res => res.id == game_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            var players = await _context.players.Where(res => res.match_id == game_id).Select(res => res.User).ToListAsync();

            if (!players.Any()) { return NotFound(new { message = "No players found in the match." }); }

            return Ok(players);
        }

        /// <summary> Retrieves all games of a specific player. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Games>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a player games",
            Description = "Fetches all games of a user. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{user_id}/Games")]    // GET: /api/Players/{user_id}/Games
        public async Task<ActionResult<IEnumerable<Users>>> GetPlayerGames(int user_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var userExists = await _context.users.AnyAsync(res => res.id == user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var games = await _context.players.Where(res => res.user_id == user_id).Select(res => res.Game).ToListAsync();

            if (!games.Any()) { return NotFound(new { message = "The player is not in any games." }); }

            return Ok(games);
        }

        /// <summary> Add players to games. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Players))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Add players to games",
            Description = "Adds a player to a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPost]  // POST: /api/Players
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

        /// <summary> Remove players of games. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Players))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Remove players of games",
            Description = "Removes a player of a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpDelete("{userId}/{matchId}")]  // DELETE: /api/Players/{userId}/{matchId}
        public async Task<ActionResult> RemovePlayer(int userId, int matchId)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!PlayerExists(userId, matchId)) { return NotFound(new { status = 404 }); }

            var player = await _context.players.FirstOrDefaultAsync(p => p.user_id == userId && p.match_id == matchId);

            if (player == null) { return NotFound(new { status = 404 }); }

            _context.players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Player removed successfully." });
        }
    }
}

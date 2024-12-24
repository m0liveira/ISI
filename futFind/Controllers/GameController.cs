using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using futFind.Swagger.Shared;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        private bool GameExists(int id) { return _context.games.Any(g => g.id == id); }

        /// <summary> Retrieves all games. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Games>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get all games",
            Description = "Fetches all games. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpGet]   // GET: /api/Games
        public async Task<ActionResult<IEnumerable<Games>>> GetGames()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.games.ToListAsync());
        }

        /// <summary> Retrieves a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a game",
            Description = "Fetches a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{id}")]   // GET: /api/Games/{id}
        public async Task<ActionResult<Games>> GetGame(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!GameExists(id)) { return NotFound(new { message = "Game not found." }); }

            return Ok(await _context.games.FindAsync(id));
        }

        /// <summary> Retrieves a game by share code. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a game by share code",
            Description = "Fetches a game by share code. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("/Code/{share_code}")]     // GET: /api/Games/{share_code}
        public async Task<ActionResult<Games>> GetGamebyCode(string share_code)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var game = await _context.games.FirstOrDefaultAsync(res => res.share_code == share_code);

            if (game == null) { return NotFound(new { message = "Game with the provided invite code was not found." }); }

            return Ok(game);
        }

        /// <summary> Creates a games. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Create a game",
            Description = "Creates a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpPost]  // POST: /api/Games
        public async Task<ActionResult<Games>> CreateGame(Games game)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            _context.games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.id }, game);
        }

        /// <summary> Updates a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Update a game",
            Description = "Updates a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPut("{id}")]   // PUT: /api/Games/{id}
        public async Task<ActionResult<Games>> UpdateGame(int id, Games game)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

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

        /// <summary> Deletes a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Delete a game",
            Description = "Deletes a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpDelete("{id}")]    // DELETE: /api/Games/{id}
        public async Task<ActionResult<Games>> DeleteGame(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!GameExists(id)) { return NotFound(new { message = "Game not found." }); }

            var game = await _context.games.FindAsync(id);
            if (game == null) { return NotFound(new { message = "Game not found." }); }

            _context.games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Game deleted successfully." });
        }
    }
}

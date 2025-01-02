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
    [Authorize]  // Requer autenticação para todas as rotas
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;  // Injeta o contexto do base de dados
        }

        // Verifica se o jogo com o ID especificado existe no base
        private bool GameExists(int id) 
        { 
            return _context.games.Any(g => g.id == id); 
        }

        /// <summary> Retrieves all games. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Games>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get all games",
            Description = "Fetches all games. Requires the `Authorization` header to be set with a valid token."
        )]
        [HttpGet]   // GET: /api/Games
        public async Task<ActionResult<IEnumerable<Games>>> GetGames()
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Verifica se o token é válido
            if (!IsValidToken(token)) 
            { 
                return Unauthorized(new { message = "Invalid token." }); 
            }

            // Retorna a lista de jogos
            return Ok(await _context.games.ToListAsync());
        }

        // Função para validar o token
        private bool IsValidToken(string token)
        {
            // Lógica para validar o token (substitua conforme sua necessidade)
            return true; // Simulação de validação, deve ser substituída pela lógica real
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
        [HttpGet("{id}")]   // GET: /api/Games/{id}
        public async Task<ActionResult<Games>> GetGame(int id)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Verifica se o jogo existe
            if (!GameExists(id)) 
            { 
                return NotFound(new { message = "Game not found." }); 
            }

            // Retorna o jogo com o ID fornecido
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
        [HttpGet("/Code/{share_code}")]     // GET: /api/Games/{share_code}
        public async Task<ActionResult<Games>> GetGamebyCode(string share_code)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Procura o jogo pelo código de compartilhamento
            var game = await _context.games.FirstOrDefaultAsync(res => res.share_code == share_code);

            // Verifica se o jogo foi encontrado
            if (game == null) 
            { 
                return NotFound(new { message = "Game with the provided invite code was not found." }); 
            }

            return Ok(game);
        }

        /// <summary> Creates a game. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Games))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Create a game",
            Description = "Creates a game. Requires the `Authorization` header to be set with a valid token."
        )]
        [HttpPost]  // POST: /api/Games
        public async Task<ActionResult<Games>> CreateGame(Games game)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Adiciona o novo jogo no contexto e guarda na base de dados
            _context.games.Add(game);
            await _context.SaveChangesAsync();

            // Retorna a ação que permite buscar o jogo recém-criado
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
        [HttpPut("{id}")]   // PUT: /api/Games/{id}
        public async Task<ActionResult<Games>> UpdateGame(int id, Games game)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Verifica se o jogo existe na base de dados
            if (!GameExists(id)) 
            { 
                return NotFound(new { message = "Game not found." }); 
            }

            // Atualiza as propriedades do jogo existente
            var existingGame = await _context.games.FindAsync(id);
            existingGame.host_id = game.host_id;
            existingGame.date = game.date;
            existingGame.address = game.address;
            existingGame.capacity = game.capacity;
            existingGame.price = game.price;
            existingGame.is_private = game.is_private;
            existingGame.share_code = game.share_code;
            existingGame.status = game.status;

            // Marca a entidade como modificada e guarda as alterações
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
        [HttpDelete("{id}")]    // DELETE: /api/Games/{id}
        public async Task<ActionResult<Games>> DeleteGame(int id)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
            { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Verifica se o jogo existe na base de dados
            if (!GameExists(id)) 
            { 
                return NotFound(new { message = "Game not found." }); 
            }

            // Remove o jogo da base de dados
            var game = await _context.games.FindAsync(id);
            _context.games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Game deleted successfully." });
        }
    }
}

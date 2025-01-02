using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;
using futFind.Swagger.Shared;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace futFind.Controllers
{
    // Define o caminho para as rotas da API. No caso, qualquer pedido para "api/Notification" será direcionado para este controlador.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Exige que o utilizador esteja autenticado para aceder a qualquer método deste controlador
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Construtor que injeta o contexto da base de dados no controlador
        public NotificationController(AppDbContext context)
        {
            _context = context;  // Inicializa o contexto da base de dados
        }

        // Método para verificar se uma notificação existe com base no ID fornecido
        private bool NotificationExists(int id) { return _context.notifications.Any(n => n.id == id); }

        /// <summary> Recupera todas as notificações. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Notifications>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get a list of notifications",
            Description = "Fetches a list of notifications. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        
        // Método GET para obter todas as notificações
        [HttpGet]   // GET: /api/Notifications
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotifications()
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Retorna a lista de notificações da base de dados
            return Ok(await _context.notifications.ToListAsync());
        }

        /// <summary> Recupera uma notificação específica. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Notifications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a notification",
            Description = "Fetches a notification. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        
        // Método GET para obter uma notificação específica usando o ID
        [HttpGet("{id}")]   // GET: /api/Notifications/{id}
        public async Task<ActionResult<Notifications>> GetNotification(int id)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se a notificação existe
            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            // Retorna a notificação encontrada
            return Ok(await _context.notifications.FindAsync(id));
        }

        /// <summary> Recupera as notificações de um jogo específico. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Notifications>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a game notifications",
            Description = "Fetches a game notifications. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        
        // Método GET para obter notificações associadas a um jogo específico
        [HttpGet("Game/{game_id}")] // GET: /api/Notifications/Game/{game_id}
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotificationsByGame(int game_id)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o jogo existe na base de dados
            var gameExists = await _context.games.AnyAsync(g => g.id == game_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            // Recupera as notificações associadas ao jogo
            var notifications = await _context.notifications.Where(n => n.match_id == game_id).ToListAsync();

            // Se não houver notificações associadas ao jogo, retorna erro
            if (!notifications.Any()) { return NotFound(new { message = "No notifications found for the specified game." }); }

            // Retorna as notificações associadas ao jogo
            return Ok(notifications);
        }

        /// <summary> Cria uma nova notificação. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Notifications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Create a notifications",
            Description = "Creates a game notification. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        
        // Método POST para criar uma nova notificação
        [HttpPost]  // POST: /api/Notifications
        public async Task<ActionResult<Notifications>> CreateNotification(Notifications notification)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Adiciona a nova notificação à base de dados
            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Retorna a notificação criada com status 201 (Created)
            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        /// <summary> Atualiza uma notificação específica. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Notifications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Update a game notifications",
            Description = "Updates a game notifications. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        
        // Método PUT para atualizar uma notificação existente
        [HttpPut("{id}")]   // PUT: /api/Notifications/{id}
        public async Task<ActionResult<Notifications>> UpdateNotification(int id, Notifications notification)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se a notificação existe
            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            var existingNotification = await _context.notifications.FindAsync(id);

            // Se a notificação não for encontrada, retorna erro
            if (existingNotification == null) { return NotFound(new { message = "Notification not found." }); }

            // Atualiza os dados da notificação
            existingNotification.match_id = notification.match_id;
            existingNotification.message = notification.message;
            existingNotification.seen = notification.seen;
            existingNotification.timestamp = notification.timestamp;

            // Marca a entidade como modificada
            _context.Entry(existingNotification).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Retorna a notificação atualizada
            return Ok(existingNotification);
        }

        /// <summary> Elimina uma notificação específica. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Notifications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Delete a game notifications",
            Description = "Deletes a game notifications. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        
        // Método DELETE para eliminar uma notificação
        [HttpDelete("{id}")]    // DELETE: /api/Notifications/{id}
        public async Task<ActionResult> DeleteNotification(int id)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se a notificação existe
            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            var notification = await _context.notifications.FindAsync(id);

            // Se a notificação não for encontrada, retorna erro
            if (notification == null) { return NotFound(new { message = "Notification not found." }); }

            // Remove a notificação da base de dados
            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();

            // Retorna a mensagem de sucesso
            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}

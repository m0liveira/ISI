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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        private bool NotificationExists(int id) { return _context.notifications.Any(n => n.id == id); }

        /// <summary> Retrieves all notifications. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Notifications>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get a list of notifications",
            Description = "Fetches a list of notifications. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpGet]   // GET: /api/Notifications
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotifications()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.notifications.ToListAsync());
        }

        /// <summary> Retrieves a specific game notification. </summary>
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
        [HttpGet("{id}")]   // GET: /api/Notifications/{id}
        public async Task<ActionResult<Notifications>> GetNotification(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            return Ok(await _context.notifications.FindAsync(id));
        }

        /// <summary> Retrieves a specific game notifications. </summary>
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
        [HttpGet("Game/{game_id}")] // GET: /api/Notifications/Game/{game_id}
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotificationsByGame(int game_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var gameExists = await _context.games.AnyAsync(g => g.id == game_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            var notifications = await _context.notifications.Where(n => n.match_id == game_id).ToListAsync();

            if (!notifications.Any()) { return NotFound(new { message = "No notifications found for the specified game." }); }

            return Ok(notifications);
        }

        /// <summary> Create a notification. </summary>
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
        [HttpPost]  // POST: /api/Notifications
        public async Task<ActionResult<Notifications>> CreateNotification(Notifications notification)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        /// <summary> Updates a specific game notifications. </summary>
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
        [HttpPut("{id}")]   // PUT: /api/Notifications/{id}
        public async Task<ActionResult<Notifications>> UpdateNotification(int id, Notifications notification)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            var existingNotification = await _context.notifications.FindAsync(id);

            if (existingNotification == null) { return NotFound(new { message = "Notification not found." }); }

            existingNotification.match_id = notification.match_id;
            existingNotification.message = notification.message;
            existingNotification.seen = notification.seen;
            existingNotification.timestamp = notification.timestamp;

            _context.Entry(existingNotification).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(existingNotification);
        }

        /// <summary> Deletes a specific game notifications. </summary>
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
        [HttpDelete("{id}")]    // DELETE: /api/Notifications/{id}
        public async Task<ActionResult> DeleteNotification(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            var notification = await _context.notifications.FindAsync(id);

            if (notification == null) { return NotFound(new { message = "Notification not found." }); }

            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}

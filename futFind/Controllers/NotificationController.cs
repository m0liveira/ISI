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
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        private bool NotificationExists(int id) { return _context.notifications.Any(n => n.id == id); }

        // GET: /api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotifications()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            return Ok(await _context.notifications.ToListAsync());
        }

        // GET: /api/Notifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notifications>> GetNotification(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            return Ok(await _context.notifications.FindAsync(id));
        }

        // GET: /api/Notifications/Game/{game_id}
        [HttpGet("Game/{game_id}")]
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotificationsByGame(int game_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var gameExists = await _context.games.AnyAsync(g => g.id == game_id);
            if (!gameExists) { return NotFound(new { message = "Game not found." }); }

            var notifications = await _context.notifications.Where(n => n.match_id == game_id).ToListAsync();

            if (!notifications.Any()) { return NotFound(new { message = "No notifications found for the specified game." }); }

            return Ok(notifications);
        }

        // POST: /api/Notifications
        [HttpPost]
        public async Task<ActionResult<Notifications>> CreateNotification(Notifications notification)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        // PUT: /api/Notifications/{id}
        [HttpPut("{id}")]
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

        // DELETE: /api/Notifications/{id}
        [HttpDelete("{id}")]
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

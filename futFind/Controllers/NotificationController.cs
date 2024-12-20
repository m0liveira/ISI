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
            return Ok(await _context.notifications.ToListAsync());
        }

        // GET: /api/Notifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notifications>> GetNotification(int id)
        {
            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            return Ok(await _context.notifications.FindAsync(id));
        }

        // POST: /api/Notifications
        [HttpPost]
        public async Task<ActionResult<Notifications>> CreateNotification(Notifications notification)
        {
            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        // PUT: /api/Notifications/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Notifications>> UpdateNotification(int id, Notifications notification)
        {
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
            if (!NotificationExists(id)) { return NotFound(new { message = "Notification not found." }); }

            var notification = await _context.notifications.FindAsync(id);

            if (notification == null) { return NotFound(new { message = "Notification not found." }); }

            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}

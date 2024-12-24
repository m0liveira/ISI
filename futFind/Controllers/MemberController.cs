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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary> Retrieves a clan member. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a clan member",
            Description = "Fetches a clan member. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{user_id}/{clan_id}")]    // GET: /api/Member/{user_id}/{clan_id} (helper method for CreatedAtAction)
        public async Task<ActionResult<Members>> GetClanMember(int user_id, int clan_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.user_id == user_id && res.clan_id == clan_id);

            if (clanMember == null) { return NotFound(); }

            return Ok(clanMember);
        }

        /// <summary> Retrieves all clan members. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Members>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get all clan members",
            Description = "Fetches all clan members. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("clan/{clan_id}")]     // GET: /api/Member/clan/{clan_id}
        public async Task<ActionResult<IEnumerable<Users>>> GetMembersOfClan(int clan_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var clanExists = await _context.teams.AnyAsync(res => res.id == clan_id);
            if (!clanExists) { return NotFound(new { message = "Clan not found." }); }

            var members = await _context.members.Where(res => res.clan_id == clan_id).Select(res => res.User).ToListAsync();

            if (!members.Any()) { return NotFound(new { message = "No members found in the specified clan." }); }

            return Ok(members);
        }

        /// <summary> Retrieves a user clan. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Teams>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a user clan",
            Description = "Fetches a user clan. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("user/{user_id}")] // GET: /api/Member/user/{user_id}
        public async Task<ActionResult<IEnumerable<Users>>> GetUserClan(int user_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var userExists = await _context.users.AnyAsync(res => res.id == user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var clans = await _context.members.Where(res => res.user_id == user_id).Select(res => res.Team).ToListAsync();

            if (!clans.Any()) { return NotFound(new { message = "User is not a member of any clan." }); }

            return Ok(clans);
        }

        /// <summary> Adds a clan member. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Add a clan member",
            Description = "Adds a clan member. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPost]  // POST: /api/Member
        public async Task<IActionResult> AddMemberToClan(Members member)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var userExists = await _context.users.AnyAsync(u => u.id == member.user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var teamExists = await _context.teams.AnyAsync(t => t.id == member.clan_id);
            if (!teamExists) { return NotFound(new { message = "Clan not found." }); }

            var memberExists = await _context.members.AnyAsync(cm => cm.user_id == member.user_id && cm.clan_id == member.clan_id);
            if (memberExists) { return Conflict(new { message = "User is already a member of the clan." }); }

            var newMember = new Members
            {
                user_id = member.user_id,
                clan_id = member.clan_id
            };

            _context.members.Add(newMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClanMember), new { user_id = newMember.user_id, clan_id = newMember.clan_id }, newMember);
        }

        /// <summary> Removes a clan member. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Remove a clan member",
            Description = "Removes a clan member. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpDelete("{clan_id}/{user_id}")]     // DELETE: /api/Member/{clan_id}/{user_id}
        public async Task<IActionResult> RemoveUserFromClan(int clan_id, int user_id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.clan_id == clan_id && res.user_id == user_id);

            if (clanMember == null) { return NotFound(new { message = "The user is not a member of the specified clan." }); }

            _context.members.Remove(clanMember);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User successfully removed from the clan." });
        }

    }
}
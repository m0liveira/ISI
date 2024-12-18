using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;

namespace futFind.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/Member/{user_id}/{clan_id} (helper method for CreatedAtAction)
        [HttpGet("{user_id}/{clan_id}")]
        public async Task<ActionResult<Members>> GetClanMember(int user_id, int clan_id)
        {
            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.user_id == user_id && res.clan_id == clan_id);

            if (clanMember == null) { return NotFound(); }

            return Ok(clanMember);
        }

        // GET: /api/Member/{clan_id}
        [HttpGet("{clan_id}")]
        public async Task<ActionResult<IEnumerable<Users>>> GetMembersOfClan(int clan_id)
        {
            var clanExists = await _context.teams.AnyAsync(res => res.id == clan_id);
            if (!clanExists) { return NotFound(new { message = "Clan not found." }); }

            var members = await _context.members.Where(res => res.clan_id == clan_id).Select(res => res.User).ToListAsync();

            if (!members.Any()) { return NotFound(new { message = "No members found in the specified clan." }); }

            return Ok(members);
        }

        // POST: /api/Member
        [HttpPost]
        public async Task<ActionResult<Members>> AddMemberToClan(Members member)
        {
            var userExists = await _context.users.AnyAsync(res => res.id == member.user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            var clanExists = await _context.teams.AnyAsync(res => res.id == member.clan_id);
            if (!clanExists) { return NotFound(new { message = "Clan not found." }); }

            var memberExists = await _context.members.AnyAsync(res => res.user_id == member.user_id && res.clan_id == member.clan_id);
            if (memberExists) { return Conflict(new { message = "User is already a member of the clan." }); }

            _context.members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClanMember), new { user_id = member.user_id, clan_id = member.clan_id }, member);
        }

        // DELETE: /api/Member/{clan_id}/{user_id}
        [HttpDelete("{clan_id}/{user_id}")]
        public async Task<IActionResult> RemoveUserFromClan(int clan_id, int user_id)
        {
            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.clan_id == clan_id && res.user_id == user_id);

            if (clanMember == null) { return NotFound(new { message = "The user is not a member of the specified clan." }); }

            _context.members.Remove(clanMember);
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(new { message = "User successfully removed from the clan." });
        }

    }
}
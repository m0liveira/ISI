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
    public class TeamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teams>>> GetTeams() { return Ok(await _context.teams.ToListAsync()); }

        // GET: /api/Teams/code/{invite_code}
        [HttpGet("code/{invite_code}")]
        public async Task<ActionResult<Teams>> GetTeamByCode(string invite_code)
        {
            var team = await _context.teams.FirstOrDefaultAsync(res => res.invite_code == invite_code);

            if (team == null) { return NotFound(new { message = "Team with the provided invite code was not found." }); }

            return Ok(team);
        }

        // GET: /api/Teams/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Teams>> GetTeamById(int id)
        {
            var team = await _context.teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // POST: /api/Teams
        [HttpPost]
        public async Task<ActionResult<Teams>> CreateTeam(Teams team)
        {
            // Check if the team name already exists
            var existingTeam = await _context.teams.FirstOrDefaultAsync(res => res.name == team.name);

            if (existingTeam != null)
            {
                return Conflict(new { message = "Team name is already in use." });
            }

            // Add the new team to the database
            _context.teams.Add(team);
            await _context.SaveChangesAsync();

            // Return the created team with a 201 Created response
            return CreatedAtAction(nameof(GetTeamById), new { id = team.id }, team);
        }

        // PUT: /api/Teams/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Teams>> UpdateTeam(int id, Teams updatedTeam)
        {
            // Find the team by ID
            var existingTeam = await _context.teams.FindAsync(id);

            if (existingTeam == null) { return NotFound(new { message = "Team not found." }); }

            // Check if the team name already exists for another team
            if (updatedTeam.name != existingTeam.name)
            {
                var duplicateName = await _context.teams.AnyAsync(res => res.name == updatedTeam.name && res.id != id);
                if (duplicateName) { return Conflict(new { message = "Team name is already in use." }); }
            }

            // Check if the invite_Code already exists for another team
            if (updatedTeam.invite_code != existingTeam.invite_code)
            {
                var duplicateCode = await _context.teams.AnyAsync(res => res.invite_code == updatedTeam.invite_code && res.id != id);

                if (duplicateCode)
                {
                    return Conflict(new { message = "Invite code is already in use." });
                }
            }

            // Update the existing team with new values
            existingTeam.name = updatedTeam.name;
            existingTeam.description = updatedTeam.description;
            existingTeam.capacity = updatedTeam.capacity;
            existingTeam.invite_code = updatedTeam.invite_code;
            existingTeam.leader = updatedTeam.leader;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating the team." });
            }

            // Return the updated team
            return Ok(existingTeam);
        }

    }
}
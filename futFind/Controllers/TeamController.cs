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

        // GET: /api/Teams/{id} (helper function for CreatedAtAction)
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
            var existingTeam = await _context.teams.FirstOrDefaultAsync(t => t.name == team.name);

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
    }
}
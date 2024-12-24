using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using futFind.Swagger.Shared;

namespace futFind.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary> Retrieves all teams. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Teams>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get a list of teams",
            Description = "Fetches a list of teams. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpGet]   // GET: /api/Team
        public async Task<ActionResult<IEnumerable<Teams>>> GetTeams()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }
            return Ok(await _context.teams.ToListAsync());
        }

        /// <summary> Retrieves a specific team by using its share code. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teams))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a team by share code",
            Description = "Fetches a team data by using a share code. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("code/{invite_code}")]   // GET: /api/Team/code/{invite_code}
        public async Task<ActionResult<Teams>> GetTeamByCode(string invite_code)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var team = await _context.teams.FirstOrDefaultAsync(res => res.invite_code == invite_code);

            if (team == null) { return NotFound(new { status = 404 }); }

            return Ok(team);
        }

        /// <summary> Retrieves a specific team by using its id. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teams))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a team by ID",
            Description = "Fetches a team data. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{id}")]   // GET: /api/Team/{id}
        public async Task<ActionResult<Teams>> GetTeamById(int id)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var team = await _context.teams.FindAsync(id);

            if (team == null) { return NotFound(new { status = 404 }); }

            return team;
        }

        /// <summary> Create a team. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teams))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Create a team",
            Description = "Creates a team. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpPost]  // POST: /api/Team
        public async Task<ActionResult<Teams>> CreateTeam(Teams team)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var existingTeam = await _context.teams.FirstOrDefaultAsync(res => res.name == team.name);

            if (existingTeam != null) { return Conflict(new { message = "Team name is already in use." }); }

            _context.teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeamById), new { id = team.id }, team);
        }

        /// <summary> Updates a specific team by using its id. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Users))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Update a user by ID",
            Description = "Updates a user data. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPut("{id}")]   // PUT: /api/Team/{id}
        public async Task<ActionResult<Teams>> UpdateTeam(int id, Teams updatedTeam)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            var existingTeam = await _context.teams.FindAsync(id);

            if (existingTeam == null) { return NotFound(new { status = 404 }); }

            if (updatedTeam.name != existingTeam.name)
            {
                var duplicateName = await _context.teams.AnyAsync(res => res.name == updatedTeam.name && res.id != id);
                if (duplicateName) { return Conflict(new { message = "Team name is already in use." }); }
            }

            if (updatedTeam.invite_code != existingTeam.invite_code)
            {
                var duplicateCode = await _context.teams.AnyAsync(res => res.invite_code == updatedTeam.invite_code && res.id != id);

                if (duplicateCode)
                {
                    return Conflict(new { message = "Invite code is already in use." });
                }
            }

            existingTeam.name = updatedTeam.name;
            existingTeam.description = updatedTeam.description;
            existingTeam.capacity = updatedTeam.capacity;
            existingTeam.invite_code = updatedTeam.invite_code;
            existingTeam.leader = updatedTeam.leader;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the team." });
            }

            return Ok(existingTeam);
        }

    }
}
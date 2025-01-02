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

        // Construtor que recebe o contexto do banco de dados
        public TeamController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary> Recupera todas as equipas. </summary>
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
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Retorna a lista de equipas
            return Ok(await _context.teams.ToListAsync());
        }

        /// <summary> Recupera uma equipa específica utilizando o código de partilha. </summary>
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
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Procura a equipa com o código de convite fornecido
            var team = await _context.teams.FirstOrDefaultAsync(res => res.invite_code == invite_code);

            // Se não encontrar a equipa, retorna erro 404
            if (team == null) { 
                return NotFound(new { status = 404 }); 
            }

            return Ok(team);
        }

        /// <summary> Recupera uma equipa específica utilizando o seu id. </summary>
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
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Procura a equipa pelo id
            var team = await _context.teams.FindAsync(id);

            // Se não encontrar a equipa, retorna erro 404
            if (team == null) { 
                return NotFound(new { status = 404 }); 
            }

            return team;
        }

        /// <summary> Cria uma nova equipa. </summary>
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
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Verifica se o nome da equipa já existe
            var existingTeam = await _context.teams.FirstOrDefaultAsync(res => res.name == team.name);
            if (existingTeam != null) { 
                return Conflict(new { message = "Team name is already in use." }); 
            }

            // Adiciona a equipa ao banco de dados e guarda
            _context.teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeamById), new { id = team.id }, team);
        }

        /// <summary> Atualiza uma equipa específica utilizando o seu id. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Teams))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Update a team by ID",
            Description = "Updates a team data. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPut("{id}")]   // PUT: /api/Team/{id}
        public async Task<ActionResult<Teams>> UpdateTeam(int id, Teams updatedTeam)
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { 
                return BadRequest(new { message = "Authorization header is missing." }); 
            }

            // Procura a equipa existente pelo id
            var existingTeam = await _context.teams.FindAsync(id);

            // Se não encontrar a equipa, retorna erro 404
            if (existingTeam == null) { 
                return NotFound(new { status = 404 }); 
            }

            // Verifica se o nome da equipa foi alterado e se já existe outro time com o mesmo nome
            if (updatedTeam.name != existingTeam.name)
            {
                var duplicateName = await _context.teams.AnyAsync(res => res.name == updatedTeam.name && res.id != id);
                if (duplicateName) { 
                    return Conflict(new { message = "Team name is already in use." }); 
                }
            }

            // Verifica se o código de convite foi alterado e se já existe outro time com o mesmo código
            if (updatedTeam.invite_code != existingTeam.invite_code)
            {
                var duplicateCode = await _context.teams.AnyAsync(res => res.invite_code == updatedTeam.invite_code && res.id != id);

                if (duplicateCode)
                {
                    return Conflict(new { message = "Invite code is already in use." });
                }
            }

            // Atualiza os dados da equipa
            existingTeam.name = updatedTeam.name;
            existingTeam.description = updatedTeam.description;
            existingTeam.capacity = updatedTeam.capacity;
            existingTeam.invite_code = updatedTeam.invite_code;
            existingTeam.leader = updatedTeam.leader;

            try
            {
                // Guarda as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the team." });
            }

            // Retorna a equipa atualizada
            return Ok(existingTeam);
        }
    }
}

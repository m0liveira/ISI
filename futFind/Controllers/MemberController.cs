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
    [Authorize]  // Define que o controlo está protegido e requer autorização para aceder
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Construtor para inicializar o contexto do banco de dados
        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary> Recupera um membro do clã. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a clan member",
            Description = "Recupera um membro do clã. Requer que o cabeçalho `Authorization` seja fornecido com um token válido."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{user_id}/{clan_id}")]    // GET: /api/Member/{user_id}/{clan_id} (método auxiliar para CreatedAtAction)
        public async Task<ActionResult<Members>> GetClanMember(int user_id, int clan_id)
        {
            // Verifica se o cabeçalho de autorização existe
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Procura o membro do clã com base nos IDs fornecidos
            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.user_id == user_id && res.clan_id == clan_id);

            // Se não encontrar, retorna um erro 404 (Não encontrado)
            if (clanMember == null) { return NotFound(); }

            return Ok(clanMember);  // Retorna o membro encontrado com sucesso
        }

        /// <summary> Recupera todos os membros de um clã. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Members>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get all clan members",
            Description = "Recupera todos os membros de um clã. Requer que o cabeçalho `Authorization` seja fornecido com um token válido."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("clan/{clan_id}")]     // GET: /api/Member/clan/{clan_id}
        public async Task<ActionResult<IEnumerable<Users>>> GetMembersOfClan(int clan_id)
        {
            // Verifica se o cabeçalho de autorização existe
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o clã existe no banco de dados
            var clanExists = await _context.teams.AnyAsync(res => res.id == clan_id);
            if (!clanExists) { return NotFound(new { message = "Clan not found." }); }

            // Recupera todos os membros do clã
            var members = await _context.members.Where(res => res.clan_id == clan_id).Select(res => res.User).ToListAsync();

            // Se não houver membros, retorna erro 404
            if (!members.Any()) { return NotFound(new { message = "No members found in the specified clan." }); }

            return Ok(members);  // Retorna a lista de membros encontrados
        }

        /// <summary> Recupera o clã de um utilizador. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Teams>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a user clan",
            Description = "Recupera o clã de um utilizador. Requer que o cabeçalho `Authorization` seja fornecido com um token válido."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("user/{user_id}")] // GET: /api/Member/user/{user_id}
        public async Task<ActionResult<IEnumerable<Users>>> GetUserClan(int user_id)
        {
            // Verifica se o cabeçalho de autorização existe
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o utilizador existe no banco de dados
            var userExists = await _context.users.AnyAsync(res => res.id == user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            // Recupera todos os clãs onde o utilizador é membro
            var clans = await _context.members.Where(res => res.user_id == user_id).Select(res => res.Team).ToListAsync();

            // Se o utilizador não estiver em nenhum clã, retorna erro 404
            if (!clans.Any()) { return NotFound(new { message = "User is not a member of any clan." }); }

            return Ok(clans);  // Retorna os clãs aos quais o utilizador pertence
        }

        /// <summary> Adiciona um membro a um clã. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Add a clan member",
            Description = "Adiciona um membro a um clã. Requer que o cabeçalho `Authorization` seja fornecido com um token válido."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpPost]  // POST: /api/Member
        public async Task<IActionResult> AddMemberToClan(Members member)
        {
            // Verifica se o cabeçalho de autorização existe
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o utilizador existe
            var userExists = await _context.users.AnyAsync(u => u.id == member.user_id);
            if (!userExists) { return NotFound(new { message = "User not found." }); }

            // Verifica se o clã existe
            var teamExists = await _context.teams.AnyAsync(t => t.id == member.clan_id);
            if (!teamExists) { return NotFound(new { message = "Clan not found." }); }

            // Verifica se o membro já existe no clã
            var memberExists = await _context.members.AnyAsync(cm => cm.user_id == member.user_id && cm.clan_id == member.clan_id);
            if (memberExists) { return Conflict(new { message = "User is already a member of the clan." }); }

            // Cria um novo membro para o clã
            var newMember = new Members
            {
                user_id = member.user_id,
                clan_id = member.clan_id
            };

            // Adiciona o membro ao clã e salva as alterações no banco de dados
            _context.members.Add(newMember);
            await _context.SaveChangesAsync();

            // Retorna o novo membro com o status Created
            return CreatedAtAction(nameof(GetClanMember), new { user_id = newMember.user_id, clan_id = newMember.clan_id }, newMember);
        }

        /// <summary> Remove um membro de um clã. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Members))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Remove a clan member",
            Description = "Remove um membro de um clã. Requer que o cabeçalho `Authorization` seja fornecido com um token válido."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpDelete("{clan_id}/{user_id}")]     // DELETE: /api/Member/{clan_id}/{user_id}
        public async Task<IActionResult> RemoveUserFromClan(int clan_id, int user_id)
        {
            // Verifica se o cabeçalho de autorização existe
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Procura o membro no clã com os IDs fornecidos
            var clanMember = await _context.members.FirstOrDefaultAsync(res => res.clan_id == clan_id && res.user_id == user_id);

            // Se o membro não existir, retorna erro 404
            if (clanMember == null) { return NotFound(new { message = "The user is not a member of the specified clan." }); }

            // Remove o membro do clã
            _context.members.Remove(clanMember);
            await _context.SaveChangesAsync();

            // Retorna sucesso com a mensagem de remoção
            return Ok(new { message = "User successfully removed from the clan." });
        }
    }
}

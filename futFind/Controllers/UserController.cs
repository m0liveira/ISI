using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using futFind.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using futFind.Swagger.Shared;

namespace futFind.Controllers
{
    // Define o controlador da API para o recurso de utilizadores
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Exige autenticação para acessar as rotas deste controlador
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Construtor que injeta o contexto da base de dados
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // Função auxiliar para verificar se um utilizador existe com base no ID
        private bool UserExists(int id) { return _context.users.Any(res => res.id == id); }

        // Função auxiliar para verificar se o e-mail já está em uso
        private bool EmailExists(string email) { return _context.users.Any(res => res.email == email); }

        // Função auxiliar para verificar se o número de telefone já está em uso
        private bool PhoneExists(string phone) { return _context.users.Any(res => res.phone == phone); }

        /// <summary> Retrieves a list of all users. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Users>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Fetches a list of all users. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [HttpGet]   // GET: /api/Users
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            // Verifica se o cabeçalho de autorização está presente na solicitação
            if (!Request.Headers.TryGetValue("Authorization", out var token))
            {
                return BadRequest(new { message = "Authorization header is missing." });
            }

            // Retorna a lista de utilizadores da base de dados
            return Ok(await _context.users.ToListAsync());
        }

        /// <summary> Retrieves a specific user by using its id. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Users))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Get a user by ID",
            Description = "Fetches a user data. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpGet("{id}")]   // GET: /api/Users/{id}
        public async Task<ActionResult<IEnumerable<Users>>> GetUser(int id)
        {
            // Verifica se o cabeçalho de autorização está presente na solicitação
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o utilizador existe
            if (!UserExists(id)) { return NotFound(new { status = 404 }); }

            // Retorna os dados do utilizador correspondente
            return Ok(await _context.users.FindAsync(id));
        }

        /// <summary> Creates a user. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Users))]
        [SwaggerOperation(
            Summary = "Create a new user",
            Description = "Creates a user and adds it to the database. Requires the `Authorization` header to be set with a valid token."
        )]
        [HttpPost]  // POST: /api/Users
        [AllowAnonymous]  // Permite a criação de utilizador sem autenticação
        public async Task<ActionResult<IEnumerable<Users>>> CreateUser(Users user)
        {
            // Verifica se o e-mail ou o telefone já estão em uso
            if (EmailExists(user.email)) { return Conflict(new { message = "Email is already in use." }); }

            if (PhoneExists(user.phone)) { return Conflict(new { message = "Phone is already in use." }); }

            // Adiciona o novo utilizador à base de dados
            _context.users.Add(user);
            await _context.SaveChangesAsync();

            // Retorna o utilizador criado com a resposta de sucesso
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        /// <summary> Updates a specific user by using its id. </summary>
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
        [HttpPut("{id}")]   // PUT: /api/Users/{id}
        public async Task<ActionResult<IEnumerable<Users>>> UpdateUser(int id, Users user)
        {
            // Verifica se o cabeçalho de autorização está presente na solicitação
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o utilizador existe
            if (!UserExists(id)) { return NotFound(new { status = 404 }); }

            // Verifica se o e-mail ou telefone já estão em uso
            if (EmailExists(user.email)) { return Conflict(new { message = "Email is already in use." }); }

            if (PhoneExists(user.phone)) { return Conflict(new { message = "Phone number is already in use." }); }

            // Atualiza os dados do utilizador
            var existingUser = await _context.users.FindAsync(id);

            if (existingUser == null) { return NotFound(new { status = 404 }); }

            existingUser.name = user.name;
            existingUser.email = user.email;
            existingUser.password = user.password;
            existingUser.phone = user.phone;

            _context.Entry(existingUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Retorna o utilizador atualizado
            return Ok(existingUser);
        }

        /// <summary> Deletes a specific user by using its id. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Users))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthorizationTokenMissingExample))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundExample))]
        [SwaggerOperation(
            Summary = "Deletes a user by ID",
            Description = "Deletes a user data from the database. Requires the `Authorization` header to be set with a valid token."
        )]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(AuthorizationTokenMissingExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundExample))]
        [HttpDelete("{id}")]    // DELETE: api/User/{id}
        public async Task<ActionResult<IEnumerable<Users>>> DeleteUser(int id)
        {
            // Verifica se o cabeçalho de autorização está presente na solicitação
            if (!Request.Headers.TryGetValue("Authorization", out var token)) { return BadRequest(new { message = "Authorization header is missing." }); }

            // Verifica se o utilizador existe
            if (!UserExists(id)) { return NotFound(new { status = 404 }); }

            // Remove o utilizador da base de dados
            var user = await _context.users.FindAsync(id);
            if (user == null) { return NotFound(new { status = 404 }); }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            // Retorna uma mensagem de sucesso
            return Ok(new { message = "User deleted successfully." });
        }
    }
}

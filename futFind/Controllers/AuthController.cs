using Microsoft.AspNetCore.Mvc;
using futFind.Services;
using futFind.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using futFind.Swagger.Requests;
using futFind.Swagger.Responses;

namespace futFind.Controllers
{
    // Define o caminho para as rotas da API. No caso, qualquer pedido para "api/Auth" será direcionado para este controlador.
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Declaração de variáveis privadas para os serviços necessários
        private readonly AuthService _AuthService;  // Serviço para autenticação
        private readonly AppDbContext _context;  // Contexto da base de dados

        // Construtor que injeta os serviços necessários no controlador
        public AuthController(AuthService authService, AppDbContext context)
        {
            _AuthService = authService;  // Inicializa o serviço de autenticação
            _context = context;  // Inicializa o contexto da base de dados
        }

        /// <summary> Autentica um utilizador através do email e password. </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UnauthorizedResponse))]
        [SwaggerOperation(Summary = "User authentication", Description = "Authenticates a user and generates a JWT token if the email and password are valid.")]
        [SwaggerRequestExample(typeof(LoginRequest), typeof(AuthRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthorizedResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(UnauthorizedResponseExample))]
        
        // Método POST para autenticar o utilizador. 
        [HttpPost]  // POST: /api/Auth
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest login)
        {
            // Pesquisa o utilizador na base de dados com o email fornecido
            var user = await _context.users.FirstOrDefaultAsync(res => res.email == login.Email);

            // Verifica se o utilizador não existe ou se a password não corresponde
            if (user == null || user.password != login.Password)
            {
                // Retorna um erro de autenticação (401 - Unauthorized) com a mensagem de erro em inglês
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Caso o utilizador seja autenticado com sucesso, gera um token JWT
            var token = _AuthService.GenerateToken(user.id.ToString(), user.email);

            // Prepara a resposta com o token e os dados do utilizador
            var response = new { token, data = user };

            // Retorna a resposta com sucesso (200 - OK) contendo o token e os dados do utilizador
            return Ok(response);
        }
    }
}

// Definição da classe que representa a estrutura dos dados recebidos no pedido de autenticação
public class LoginRequest
{
    // Propriedade que define o email do utilizador
    public required string Email { get; set; }
    
    // Propriedade que define a password do utilizador
    public required string Password { get; set; }
}

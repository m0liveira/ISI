using futFind.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using futFind.Services;
using Swashbuckle.AspNetCore.Filters;
using System.ServiceModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do servidor Kestrel com suporte para SOAP e HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5007); // Porta para o serviço SOAP
    options.ListenAnyIP(5008, listenOptions =>
    {
        listenOptions.UseHttps(); // Porta para HTTPS
    });
});

// Configuração do contexto do banco de dados
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração dos serviços da aplicação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "futFind API",
        Version = "v1",
        Description = "API para gerir jogos e utilizadores no futFind."
    });

    // Configuração de autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Insira o token JWT no formato: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.EnableAnnotations(); // Suporte a anotações no Swagger
    c.ExampleFilters(); // Exemplos adicionais no Swagger
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// Configuração do JWT para autenticação
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is not configured.")))
        };

        // Eventos para log durante a autenticação
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Falha na autenticação: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity?.Name != null)
                {
                    Console.WriteLine($"Token validado para o utilizador: {context.Principal.Identity.Name}");
                }
                else
                {
                    Console.WriteLine("Token validado, mas o nome do utilizador não está disponível.");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Configuração de CORS para permitir todas as origens
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do suporte a SOAP
builder.Services.AddSoapCore();
builder.Services.AddSingleton<ISoapService, SoapService>();

var app = builder.Build();

// Configuração do pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "futFind API v1"); });
}

app.UseCors("AllowAllOrigins"); // Aplicação da política de CORS
app.UseHttpsRedirection(); // Redirecionamento para HTTPS
app.UseAuthentication(); // Middleware de autenticação
app.UseAuthorization(); // Middleware de autorização
app.MapControllers(); // Mapeamento de controladores

// Configuração do endpoint para o serviço SOAP
app.UseSoapEndpoint<ISoapService>("/SoapService.svc", new BasicHttpBinding(), SoapSerializer.XmlSerializer);

app.Run(); // Execução da aplicação

// Classe principal para suportar testes
public partial class Program { }

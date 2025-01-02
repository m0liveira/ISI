using Microsoft.EntityFrameworkCore;

namespace futFind.Models
{
    // Define o contexto da base de dados da aplicação
    public class AppDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração do DbContext e as passa para a classe base
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Definir as tabelas que serão mapeadas na base de dados
        // A propriedade DbSet é responsável por mapear as entidades para as tabelas correspondentes

        // Tabela de utilizadores (users)
        public DbSet<Users> users { get; set; }

        // Tabela de equipas (teams)
        public DbSet<Teams> teams { get; set; }

        // Tabela de membros de equipas (members), que associa utilizadores a equipas
        public DbSet<Members> members { get; set; }

        // Tabela de jogos (games)
        public DbSet<Games> games { get; set; }

        // Tabela de jogadores (players), que associa utilizadores a jogos
        public DbSet<Players> players { get; set; }

        // Tabela de notificações (notifications) associadas a jogos
        public DbSet<Notifications> notifications { get; set; }
    }
}

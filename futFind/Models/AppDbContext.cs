using Microsoft.EntityFrameworkCore;
using futFind.Models;

namespace futFind.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Definir tabelas
        public DbSet<Users> users { get; set; }
        public DbSet<Teams> teams { get; set; }
        public DbSet<Members> members { get; set; }
        public DbSet<Games> games { get; set; }
        public DbSet<Players> players { get; set; }
        public DbSet<Notifications> notifications { get; set; }

    }
}

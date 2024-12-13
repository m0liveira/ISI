using Microsoft.EntityFrameworkCore;
using futFind.Models;

namespace futFind.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define as tabelas que serão mapeadas
        public DbSet<Users> users { get; set; }
    }
}

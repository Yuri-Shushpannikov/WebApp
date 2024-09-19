using Microsoft.EntityFrameworkCore;
using WebApp.Backend.Models;

namespace WebApp.Backend.Data
{
    public class WebAppContext : DbContext
    {
        public WebAppContext(DbContextOptions<WebAppContext> options)
            : base(options)
        {
        }
        public DbSet<Subdivision> Subdivision { get; set; } = default!;
        public DbSet<Worker> Worker { get; set; } = default!;
    }
}

using Microsoft.EntityFrameworkCore;
using Veenhoop.Models;

namespace Veenhoop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)                
        {
        }

        public DbSet<Gebruikers> Gebruikers { get; set; }
    }
}

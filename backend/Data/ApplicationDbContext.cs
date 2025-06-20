using Microsoft.EntityFrameworkCore;
using Veenhoop.Models;

namespace Veenhoop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)                
        {
        }

        public DbSet<Docenten> Docenten { get; set; }
        public DbSet<Klassen> Klassen { get; set; }
        public DbSet<Gebruikers> Gebruikers { get; set; }
        public DbSet<Vakken> Vakken { get; set; }
        public DbSet<Toetsen> Toetsen { get; set; }
        public DbSet<Cijfers> Cijfers { get; set; }
    }

}
        
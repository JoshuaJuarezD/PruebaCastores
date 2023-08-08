using Microsoft.EntityFrameworkCore;
using PruebaCastores.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Noticia> Noticias { get; set; }

    // Otros DbSet para otras tablas...

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
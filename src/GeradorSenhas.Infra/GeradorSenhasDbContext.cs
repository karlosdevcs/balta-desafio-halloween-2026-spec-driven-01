using GeradorSenhas.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeradorSenhas.Infra;

public sealed class GeradorSenhasDbContext(DbContextOptions<GeradorSenhasDbContext> options) : DbContext(options)
{
    public DbSet<Senha> Senhas => Set<Senha>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorSenhasDbContext).Assembly);
    }
}

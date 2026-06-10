using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JornadaDaTerra.Api.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core. Mapeia o domínio da "Jornada da Terra"
/// para o banco relacional (Oracle) e aplica todas as configurações Fluent API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Produtor> Produtores => Set<Produtor>();
    public DbSet<Fazenda> Fazendas => Set<Fazenda>();
    public DbSet<Setor> Setores => Set<Setor>();
    public DbSet<LeituraSatelite> LeiturasSatelite => Set<LeituraSatelite>();
    public DbSet<Missao> Missoes => Set<Missao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Carrega automaticamente todas as classes IEntityTypeConfiguration deste assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JornadaDaTerra.Api.Infrastructure.Data.Configurations;

public class SetorConfiguration : IEntityTypeConfiguration<Setor>
{
    public void Configure(EntityTypeBuilder<Setor> builder)
    {
        builder.ToTable("SETORES");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(s => s.Cultura)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(s => s.AreaHectares).IsRequired();

        builder.HasIndex(s => s.FazendaId);

        // 1:N — Setor -> Leituras de satélite.
        builder.HasMany(s => s.Leituras)
            .WithOne(l => l.Setor)
            .HasForeignKey(l => l.SetorId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:N — Setor -> Missões.
        builder.HasMany(s => s.Missoes)
            .WithOne(m => m.Setor)
            .HasForeignKey(m => m.SetorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

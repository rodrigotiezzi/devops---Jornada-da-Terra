using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JornadaDaTerra.Api.Infrastructure.Data.Configurations;

public class ProdutorConfiguration : IEntityTypeConfiguration<Produtor>
{
    public void Configure(EntityTypeBuilder<Produtor> builder)
    {
        builder.ToTable("PRODUTORES");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(180);

        builder.HasIndex(p => p.Email).IsUnique();

        builder.Property(p => p.Pontos).IsRequired();
        builder.Property(p => p.Nivel).IsRequired();
        builder.Property(p => p.CriadoEm).IsRequired();

        // 1:N — Produtor -> Fazendas (apaga as fazendas ao apagar o produtor).
        builder.HasMany(p => p.Fazendas)
            .WithOne(f => f.Produtor)
            .HasForeignKey(f => f.ProdutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

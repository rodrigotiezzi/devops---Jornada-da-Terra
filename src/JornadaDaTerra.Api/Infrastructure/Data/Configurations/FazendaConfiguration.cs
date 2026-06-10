using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JornadaDaTerra.Api.Infrastructure.Data.Configurations;

public class FazendaConfiguration : IEntityTypeConfiguration<Fazenda>
{
    public void Configure(EntityTypeBuilder<Fazenda> builder)
    {
        builder.ToTable("FAZENDAS");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(f => f.Municipio)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(f => f.Estado)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(f => f.AreaHectares).IsRequired();
        builder.Property(f => f.Latitude).IsRequired();
        builder.Property(f => f.Longitude).IsRequired();

        builder.HasIndex(f => f.ProdutorId);

        // 1:N — Fazenda -> Setores.
        builder.HasMany(f => f.Setores)
            .WithOne(s => s.Fazenda)
            .HasForeignKey(s => s.FazendaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

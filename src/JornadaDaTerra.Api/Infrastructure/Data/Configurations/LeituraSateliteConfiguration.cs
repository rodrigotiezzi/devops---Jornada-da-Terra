using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JornadaDaTerra.Api.Infrastructure.Data.Configurations;

public class LeituraSateliteConfiguration : IEntityTypeConfiguration<LeituraSatelite>
{
    public void Configure(EntityTypeBuilder<LeituraSatelite> builder)
    {
        builder.ToTable("LEITURAS_SATELITE");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.DataLeitura).IsRequired();
        builder.Property(l => l.TemperaturaC).IsRequired();
        builder.Property(l => l.UmidadeRelativa).IsRequired();
        builder.Property(l => l.Ndvi).IsRequired();
        builder.Property(l => l.PrecipitacaoMm).IsRequired();

        // Enum persistido como número (NUMBER) no Oracle.
        builder.Property(l => l.RiscoCalculado)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(l => l.SetorId);
    }
}

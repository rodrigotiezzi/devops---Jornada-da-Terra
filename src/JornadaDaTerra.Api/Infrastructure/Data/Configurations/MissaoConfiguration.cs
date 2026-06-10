using JornadaDaTerra.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JornadaDaTerra.Api.Infrastructure.Data.Configurations;

public class MissaoConfiguration : IEntityTypeConfiguration<Missao>
{
    public void Configure(EntityTypeBuilder<Missao> builder)
    {
        builder.ToTable("MISSOES");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Titulo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Descricao)
            .IsRequired()
            .HasMaxLength(600);

        builder.Property(m => m.Tipo).HasConversion<int>().IsRequired();
        builder.Property(m => m.Prioridade).HasConversion<int>().IsRequired();
        builder.Property(m => m.Status).HasConversion<int>().IsRequired();

        builder.Property(m => m.RecompensaPontos).IsRequired();
        builder.Property(m => m.CriadaEm).IsRequired();

        builder.HasIndex(m => m.SetorId);
        builder.HasIndex(m => m.Status);

        // N:1 opcional — Missão -> Leitura que a originou.
        // SET NULL: ao apagar a leitura, a missão é mantida (preserva o histórico) e
        // apenas perde o vínculo. Também evita conflito de múltiplos caminhos de cascade
        // (Setor -> Missão e Setor -> Leitura -> Missão) no Oracle.
        builder.HasOne(m => m.LeituraSatelite)
            .WithMany()
            .HasForeignKey(m => m.LeituraSateliteId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

using JornadaDaTerra.Api.Application.DTOs;
using JornadaDaTerra.Api.Application.Services;
using JornadaDaTerra.Api.Domain.Entities;
using JornadaDaTerra.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace JornadaDaTerra.Api.Infrastructure.Data;

/// <summary>
/// Popula o banco com dados de demonstração na primeira execução (apenas se estiver vazio).
/// Facilita a demonstração de ponta a ponta exigida na entrega.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILeituraSateliteService leituraService)
    {
        // COUNT(*) > 0 em vez de AnyAsync() por compatibilidade com o Oracle (ver Repository.ExistsAsync).
        if (await context.Produtores.CountAsync() > 0)
            return;

        var produtor = new Produtor
        {
            Nome = "João da Silva",
            Email = "joao.silva@fazenda.com.br",
            Pontos = 0,
            Nivel = 1,
            Fazendas = new List<Fazenda>
            {
                new()
                {
                    Nome = "Sítio Boa Esperança",
                    Municipio = "Ribeirão Preto",
                    Estado = "SP",
                    AreaHectares = 320,
                    Latitude = -21.1775,
                    Longitude = -47.8103,
                    Setores = new List<Setor>
                    {
                        new() { Nome = "Setor Sul", Cultura = "Soja", AreaHectares = 120 },
                        new() { Nome = "Setor Norte", Cultura = "Milho", AreaHectares = 95 }
                    }
                }
            }
        };

        context.Produtores.Add(produtor);
        await context.SaveChangesAsync();

        var setorSul = produtor.Fazendas.First().Setores.First();

        // Gera uma leitura crítica (geada) que dispara automaticamente uma missão.
        await leituraService.RegistrarAsync(new CreateLeituraSateliteDto
        {
            SetorId = setorSul.Id,
            TemperaturaC = -1.5,
            UmidadeRelativa = 88,
            Ndvi = 0.62,
            PrecipitacaoMm = 0
        });
    }
}

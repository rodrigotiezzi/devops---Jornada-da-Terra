using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JornadaDaTerra.Api.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PRODUTORES",
            columns: table => new
            {
                Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "NVARCHAR2(180)", maxLength: 180, nullable: false),
                Pontos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                Nivel = table.Column<int>(type: "NUMBER(10)", nullable: false),
                CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PRODUTORES", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FAZENDAS",
            columns: table => new
            {
                Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                Municipio = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                Estado = table.Column<string>(type: "NCHAR(2)", fixedLength: true, maxLength: 2, nullable: false),
                AreaHectares = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                ProdutorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FAZENDAS", x => x.Id);
                table.ForeignKey(
                    name: "FK_FAZENDAS_PRODUTORES_ProdutorId",
                    column: x => x.ProdutorId,
                    principalTable: "PRODUTORES",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SETORES",
            columns: table => new
            {
                Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                Cultura = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                AreaHectares = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                FazendaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SETORES", x => x.Id);
                table.ForeignKey(
                    name: "FK_SETORES_FAZENDAS_FazendaId",
                    column: x => x.FazendaId,
                    principalTable: "FAZENDAS",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "LEITURAS_SATELITE",
            columns: table => new
            {
                Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                DataLeitura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                TemperaturaC = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                UmidadeRelativa = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                Ndvi = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                PrecipitacaoMm = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                RiscoCalculado = table.Column<int>(type: "NUMBER(10)", nullable: false),
                SetorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LEITURAS_SATELITE", x => x.Id);
                table.ForeignKey(
                    name: "FK_LEITURAS_SATELITE_SETORES_SetorId",
                    column: x => x.SetorId,
                    principalTable: "SETORES",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MISSOES",
            columns: table => new
            {
                Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                Titulo = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                Descricao = table.Column<string>(type: "NVARCHAR2(600)", maxLength: 600, nullable: false),
                Tipo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                Prioridade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                RecompensaPontos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                CriadaEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                PrazoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                ConcluidaEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                SetorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                LeituraSateliteId = table.Column<int>(type: "NUMBER(10)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MISSOES", x => x.Id);
                table.ForeignKey(
                    name: "FK_MISSOES_LEITURAS_SATELITE_LeituraSateliteId",
                    column: x => x.LeituraSateliteId,
                    principalTable: "LEITURAS_SATELITE",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_MISSOES_SETORES_SetorId",
                    column: x => x.SetorId,
                    principalTable: "SETORES",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_FAZENDAS_ProdutorId",
            table: "FAZENDAS",
            column: "ProdutorId");

        migrationBuilder.CreateIndex(
            name: "IX_LEITURAS_SATELITE_SetorId",
            table: "LEITURAS_SATELITE",
            column: "SetorId");

        migrationBuilder.CreateIndex(
            name: "IX_MISSOES_LeituraSateliteId",
            table: "MISSOES",
            column: "LeituraSateliteId");

        migrationBuilder.CreateIndex(
            name: "IX_MISSOES_SetorId",
            table: "MISSOES",
            column: "SetorId");

        migrationBuilder.CreateIndex(
            name: "IX_MISSOES_Status",
            table: "MISSOES",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_PRODUTORES_Email",
            table: "PRODUTORES",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_SETORES_FazendaId",
            table: "SETORES",
            column: "FazendaId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "MISSOES");
        migrationBuilder.DropTable(name: "LEITURAS_SATELITE");
        migrationBuilder.DropTable(name: "SETORES");
        migrationBuilder.DropTable(name: "FAZENDAS");
        migrationBuilder.DropTable(name: "PRODUTORES");
    }
}

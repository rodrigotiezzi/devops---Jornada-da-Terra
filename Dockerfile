# ============================================================
# STAGE 1 — BUILD
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY src/JornadaDaTerra.Api/JornadaDaTerra.Api.csproj ./src/JornadaDaTerra.Api/
RUN dotnet restore ./src/JornadaDaTerra.Api/JornadaDaTerra.Api.csproj

COPY src/ ./src/
RUN dotnet publish ./src/JornadaDaTerra.Api/JornadaDaTerra.Api.csproj \
    -c Release -o /app/publish --no-restore

# ============================================================
# STAGE 2 — RUNTIME
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Diretório de trabalho (critério 2.1)
WORKDIR /app

# Usuário não-root (critério 2.1)
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app

USER appuser

# Expor porta (critério 2.1)
EXPOSE 8080

# Variáveis de ambiente (critério 2.1)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "JornadaDaTerra.Api.dll"]

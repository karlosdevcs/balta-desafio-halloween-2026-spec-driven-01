using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace GeradorSenhas.IntegrationTests;

/// <summary>
/// Sobe a Api em memória apontando para um PostgreSQL 18 descartável (D-12).
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _banco = new PostgreSqlBuilder("postgres:18").Build();

    public async Task InitializeAsync() => await _banco.StartAsync();

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _banco.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testes");
        builder.UseSetting("ConnectionStrings:GeradorSenhas", _banco.GetConnectionString());
    }
}

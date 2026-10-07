using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorSenhas.Infra.Migracoes;

public static class AplicacaoDeMigracoes
{
    /// <summary>
    /// Aplica as migrações pendentes do banco (D-10).
    /// </summary>
    public static async Task AplicarMigracoesAsync(this IServiceProvider services)
    {
        await using var escopo = services.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<GeradorSenhasDbContext>();
        await contexto.Database.MigrateAsync();
    }
}

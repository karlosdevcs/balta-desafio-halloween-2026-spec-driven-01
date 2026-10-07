using GeradorSenhas.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorSenhas.Infra;

public static class InjecaoDeDependencia
{
    public static IServiceCollection AdicionarInfra(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GeradorSenhasDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ISenhaRepositorio, SenhaRepositorio>();
        return services;
    }
}

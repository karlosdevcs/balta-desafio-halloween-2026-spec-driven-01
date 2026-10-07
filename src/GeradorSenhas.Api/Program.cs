using GeradorSenhas.Api;
using GeradorSenhas.Application;
using GeradorSenhas.Infra;
using GeradorSenhas.Infra.Migracoes;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GeradorSenhas")
    ?? throw new InvalidOperationException("Connection string 'GeradorSenhas' não configurada.");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeExcecoes>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<SenhaServico>();
builder.Services.AdicionarInfra(connectionString);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// D-10: migrações aplicadas na inicialização.
await app.Services.AplicarMigracoesAsync();

app.MapearEndpointsDeSenhas();

await app.RunAsync();

public partial class Program;

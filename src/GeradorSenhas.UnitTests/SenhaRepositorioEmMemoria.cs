using GeradorSenhas.Application;
using GeradorSenhas.Domain;

namespace GeradorSenhas.UnitTests;

internal sealed class SenhaRepositorioEmMemoria : ISenhaRepositorio
{
    public Dictionary<Guid, Senha> Senhas { get; } = [];

    public Task AdicionarAsync(Senha senha, CancellationToken cancellationToken)
    {
        Senhas.Add(senha.Id, senha);
        return Task.CompletedTask;
    }

    public Task<Senha?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Senhas.GetValueOrDefault(id));
}

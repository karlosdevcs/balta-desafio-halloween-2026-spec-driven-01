using GeradorSenhas.Domain;

namespace GeradorSenhas.Application;

public interface ISenhaRepositorio
{
    Task AdicionarAsync(Senha senha, CancellationToken cancellationToken);

    Task<Senha?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
}

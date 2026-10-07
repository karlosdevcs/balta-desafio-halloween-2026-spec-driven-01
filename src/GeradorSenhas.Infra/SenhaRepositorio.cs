using GeradorSenhas.Application;
using GeradorSenhas.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeradorSenhas.Infra;

internal sealed class SenhaRepositorio(GeradorSenhasDbContext contexto) : ISenhaRepositorio
{
    public async Task AdicionarAsync(Senha senha, CancellationToken cancellationToken)
    {
        contexto.Senhas.Add(senha);
        await contexto.SaveChangesAsync(cancellationToken);
    }

    public Task<Senha?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        contexto.Senhas.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}

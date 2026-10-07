using GeradorSenhas.Domain;

namespace GeradorSenhas.Application;

public sealed class SenhaServico(ISenhaRepositorio repositorio, TimeProvider relogio)
{
    public async Task<SenhaResposta> GerarAsync(int? tamanho, CancellationToken cancellationToken = default)
    {
        var tamanhoEfetivo = tamanho ?? PoliticaDeSenha.TamanhoPadrao;

        if (!PoliticaDeSenha.TamanhoEhValido(tamanhoEfetivo))
            throw new ErroDeValidacaoException(
                "tamanho",
                $"O tamanho deve estar entre {PoliticaDeSenha.TamanhoMinimo} e {PoliticaDeSenha.TamanhoMaximo}.");

        var senha = Senha.Criar(GeradorDeSenha.Gerar(tamanhoEfetivo), relogio.GetUtcNow());
        await repositorio.AdicionarAsync(senha, cancellationToken);

        return SenhaResposta.De(senha);
    }

    public async Task<SenhaResposta> ObterAsync(string? id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid))
            throw new ErroDeValidacaoException("id", "O identificador informado não é um GUID válido.");

        var senha = await repositorio.ObterPorIdAsync(guid, cancellationToken)
            ?? throw new SenhaNaoEncontradaException(guid);

        return SenhaResposta.De(senha);
    }
}

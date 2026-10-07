using GeradorSenhas.Application;
using GeradorSenhas.Domain;

namespace GeradorSenhas.UnitTests;

public class SenhaServicoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 31, 23, 59, 0, TimeSpan.Zero);

    private readonly SenhaRepositorioEmMemoria _repositorio = new();
    private readonly SenhaServico _servico;

    public SenhaServicoTests()
    {
        _servico = new SenhaServico(_repositorio, new RelogioFixo(Agora));
    }

    [Fact]
    public async Task GerarAsync_TamanhoNaoInformado_GeraSenhaCom16Caracteres()
    {
        var resposta = await _servico.GerarAsync(null);

        Assert.Equal(16, resposta.Senha.Length);
    }

    [Theory]
    [InlineData(16)]
    [InlineData(50)]
    [InlineData(128)]
    public async Task GerarAsync_TamanhoValido_GeraSenhaValidaComTamanhoPedido(int tamanho)
    {
        var resposta = await _servico.GerarAsync(tamanho);

        Assert.Equal(tamanho, resposta.Senha.Length);
        Assert.True(PoliticaDeSenha.EhValida(resposta.Senha));
    }

    [Fact]
    public async Task GerarAsync_SenhaGerada_ArmazenaERetornaIdEDataDeGeracao()
    {
        var resposta = await _servico.GerarAsync(20);

        var armazenada = Assert.Single(_repositorio.Senhas.Values);
        Assert.Equal(armazenada.Id, resposta.Id);
        Assert.Equal(armazenada.Valor, resposta.Senha);
        Assert.Equal(Agora, resposta.CriadaEm);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(129)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GerarAsync_TamanhoForaDosLimites_LancaErroDeValidacaoENaoArmazena(int tamanho)
    {
        var excecao = await Assert.ThrowsAsync<ErroDeValidacaoException>(() => _servico.GerarAsync(tamanho));

        Assert.Equal("tamanho", excecao.Campo);
        Assert.Empty(_repositorio.Senhas);
    }

    [Fact]
    public async Task ObterAsync_IdExistente_RetornaMesmaSenha()
    {
        var gerada = await _servico.GerarAsync(null);

        var consultada = await _servico.ObterAsync(gerada.Id.ToString());

        Assert.Equal(gerada, consultada);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nao-e-um-guid")]
    [InlineData("12345")]
    public async Task ObterAsync_IdEmFormatoInvalido_LancaErroDeValidacao(string? id)
    {
        var excecao = await Assert.ThrowsAsync<ErroDeValidacaoException>(() => _servico.ObterAsync(id));

        Assert.Equal("id", excecao.Campo);
    }

    [Fact]
    public async Task ObterAsync_IdInexistente_LancaSenhaNaoEncontrada()
    {
        await Assert.ThrowsAsync<SenhaNaoEncontradaException>(() => _servico.ObterAsync(Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task ObterAsync_IdVazio_LancaSenhaNaoEncontrada()
    {
        await Assert.ThrowsAsync<SenhaNaoEncontradaException>(() => _servico.ObterAsync(Guid.Empty.ToString()));
    }
}

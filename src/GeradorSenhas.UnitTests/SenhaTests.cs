using GeradorSenhas.Domain;

namespace GeradorSenhas.UnitTests;

public class SenhaTests
{
    private static readonly DateTimeOffset Agora = new(2026, 10, 31, 23, 59, 0, TimeSpan.Zero);

    [Fact]
    public void Criar_SenhaValida_RetornaEntidadeComIdVersao7()
    {
        var senha = Senha.Criar("Abcdefgh1234567!", Agora);

        Assert.NotEqual(Guid.Empty, senha.Id);
        Assert.Equal(7, senha.Id.Version);
        Assert.Equal("Abcdefgh1234567!", senha.Valor);
        Assert.Equal(Agora, senha.CriadaEm);
    }

    [Fact]
    public void Criar_DuasSenhas_GeraIdsDiferentes()
    {
        var primeira = Senha.Criar("Abcdefgh1234567!", Agora);
        var segunda = Senha.Criar("Abcdefgh1234567!", Agora);

        Assert.NotEqual(primeira.Id, segunda.Id);
    }

    [Fact]
    public void Criar_DataComPrecisaoAbaixoDeMicrossegundo_TruncaParaMicrossegundos()
    {
        var comTicksExtras = Agora.AddTicks(1_234_567);

        var senha = Senha.Criar("Abcdefgh1234567!", comTicksExtras);

        Assert.Equal(Agora.AddTicks(1_234_560), senha.CriadaEm);
    }

    [Fact]
    public void Criar_SenhaInvalida_LancaSenhaInvalidaException()
    {
        var excecao = Assert.Throws<SenhaInvalidaException>(() => Senha.Criar("curta", Agora));

        Assert.NotEmpty(excecao.Erros);
    }
}

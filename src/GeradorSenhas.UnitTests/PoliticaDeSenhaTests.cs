using GeradorSenhas.Domain;

namespace GeradorSenhas.UnitTests;

public class PoliticaDeSenhaTests
{
    private const string SenhaValida = "Abcdefgh1234567!";

    [Fact]
    public void Validar_SenhaQueAtendeTodasAsRegras_RetornaSemErros()
    {
        Assert.Empty(PoliticaDeSenha.Validar(SenhaValida));
        Assert.True(PoliticaDeSenha.EhValida(SenhaValida));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validar_SenhaVazia_RetornaErro(string? valor)
    {
        Assert.False(PoliticaDeSenha.EhValida(valor));
    }

    [Fact]
    public void Validar_SenhaCom15Caracteres_RetornaErroDeTamanho()
    {
        var erros = PoliticaDeSenha.Validar("Abcdefgh123456!");

        Assert.Contains(erros, e => e.Contains("entre 16 e 128"));
    }

    [Fact]
    public void Validar_SenhaCom129Caracteres_RetornaErroDeTamanho()
    {
        var erros = PoliticaDeSenha.Validar(SenhaValida + new string('a', 113));

        Assert.Contains(erros, e => e.Contains("entre 16 e 128"));
    }

    [Fact]
    public void Validar_SenhaCom128Caracteres_RetornaSemErros()
    {
        Assert.Empty(PoliticaDeSenha.Validar(SenhaValida + new string('a', 112)));
    }

    [Theory]
    [InlineData("Abcdefg h1234567!")]
    [InlineData("Abcdefg\th1234567!")]
    [InlineData("Abcdefg\nh1234567!")]
    public void Validar_SenhaComEspacoEmBranco_RetornaErro(string valor)
    {
        Assert.Contains(PoliticaDeSenha.Validar(valor), e => e.Contains("espaços em branco"));
    }

    [Fact]
    public void Validar_SenhaSemCaractereEspecial_RetornaErro()
    {
        Assert.Contains(PoliticaDeSenha.Validar("Abcdefgh12345678"), e => e.Contains("caractere especial"));
    }

    [Fact]
    public void Validar_SenhaSemMaiuscula_RetornaErro()
    {
        Assert.Contains(PoliticaDeSenha.Validar("abcdefgh1234567!"), e => e.Contains("maiúscula"));
    }

    [Fact]
    public void Validar_SenhaSemMinuscula_RetornaErro()
    {
        Assert.Contains(PoliticaDeSenha.Validar("ABCDEFGH1234567!"), e => e.Contains("minúscula"));
    }

    [Fact]
    public void Validar_SenhaSemDigito_RetornaErro()
    {
        Assert.Contains(PoliticaDeSenha.Validar("Abcdefghijklmno!"), e => e.Contains("dígito"));
    }

    [Theory]
    [InlineData("Abcdefgh1234567!é")]
    [InlineData("Abcdefgh1234567!\"")]
    [InlineData("Abcdefgh1234567!\\")]
    public void Validar_SenhaComCaractereForaDoAlfabeto_RetornaErro(string valor)
    {
        Assert.Contains(PoliticaDeSenha.Validar(valor), e => e.Contains("fora do alfabeto"));
    }

    [Theory]
    [InlineData(15, false)]
    [InlineData(16, true)]
    [InlineData(128, true)]
    [InlineData(129, false)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void TamanhoEhValido_ValoresNosLimites_RetornaEsperado(int tamanho, bool esperado)
    {
        Assert.Equal(esperado, PoliticaDeSenha.TamanhoEhValido(tamanho));
    }
}

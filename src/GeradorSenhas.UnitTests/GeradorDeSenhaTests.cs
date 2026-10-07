using GeradorSenhas.Domain;

namespace GeradorSenhas.UnitTests;

public class GeradorDeSenhaTests
{
    private const int Quantidade = 1_000;

    [Theory]
    [InlineData(16)]
    [InlineData(64)]
    [InlineData(128)]
    public void Gerar_TamanhoValido_SenhasAtendemPoliticaETamanho(int tamanho)
    {
        for (var i = 0; i < Quantidade; i++)
        {
            var senha = GeradorDeSenha.Gerar(tamanho);

            Assert.Equal(tamanho, senha.Length);
            Assert.Empty(PoliticaDeSenha.Validar(senha));
        }
    }

    [Theory]
    [InlineData(16)]
    [InlineData(64)]
    [InlineData(128)]
    public void Gerar_VariasSenhas_NaoSeRepetem(int tamanho)
    {
        var senhas = Enumerable.Range(0, Quantidade).Select(_ => GeradorDeSenha.Gerar(tamanho)).ToHashSet();

        Assert.Equal(Quantidade, senhas.Count);
    }

    [Fact]
    public void Gerar_VariasSenhas_PrimeiroCaractereNaoSegueOrdemFixaDeCategorias()
    {
        var primeirosCaracteres = Enumerable.Range(0, Quantidade)
            .Select(_ => GeradorDeSenha.Gerar(16)[0])
            .ToList();

        Assert.Contains(primeirosCaracteres, c => PoliticaDeSenha.LetrasMaiusculas.Contains(c));
        Assert.Contains(primeirosCaracteres, c => PoliticaDeSenha.LetrasMinusculas.Contains(c));
        Assert.Contains(primeirosCaracteres, c => PoliticaDeSenha.Digitos.Contains(c));
        Assert.Contains(primeirosCaracteres, c => PoliticaDeSenha.CaracteresEspeciais.Contains(c));
    }

    [Theory]
    [InlineData(15)]
    [InlineData(129)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Gerar_TamanhoForaDosLimites_LancaArgumentOutOfRangeException(int tamanho)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GeradorDeSenha.Gerar(tamanho));
    }
}

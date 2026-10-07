using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace GeradorSenhas.IntegrationTests;

public class EndpointsDeSenhasTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string TipoProblemDetails = "application/problem+json";

    private readonly HttpClient _cliente = factory.CreateClient();

    [Fact]
    public async Task PostSenhas_SemCorpo_Retorna201ComSenhaDe16Caracteres()
    {
        var resposta = await _cliente.PostAsync("/senhas", null);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<SenhaRespostaJson>();
        Assert.NotNull(corpo);
        Assert.Equal(16, corpo.Senha.Length);
        Assert.Equal($"/senhas/{corpo.Id}", resposta.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task PostSenhas_ComTamanho_Retorna201ComTamanhoPedidoEJsonCamelCase()
    {
        var resposta = await _cliente.PostAsJsonAsync("/senhas", new { tamanho = 40 });

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(40, json.RootElement.GetProperty("senha").GetString()!.Length);
        Assert.True(json.RootElement.TryGetProperty("id", out _));
        Assert.True(json.RootElement.TryGetProperty("criadaEm", out _));
    }

    [Theory]
    [InlineData(15)]
    [InlineData(129)]
    public async Task PostSenhas_TamanhoForaDosLimites_Retorna400ComProblemDetails(int tamanho)
    {
        var resposta = await _cliente.PostAsJsonAsync("/senhas", new { tamanho });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal(TipoProblemDetails, resposta.Content.Headers.ContentType?.MediaType);
        var problema = await resposta.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problema);
        Assert.Contains("tamanho", problema.Errors.Keys);
    }

    [Fact]
    public async Task PostSenhas_TamanhoNaoNumerico_Retorna400ComProblemDetails()
    {
        var conteudo = new StringContent("""{ "tamanho": "abc" }""", Encoding.UTF8, "application/json");

        var resposta = await _cliente.PostAsync("/senhas", conteudo);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal(TipoProblemDetails, resposta.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetSenhas_IdDeSenhaCriada_Retorna200ComMesmaSenha()
    {
        var criacao = await _cliente.PostAsJsonAsync("/senhas", new { tamanho = 32 });
        var criada = await criacao.Content.ReadFromJsonAsync<SenhaRespostaJson>();
        Assert.NotNull(criada);

        var resposta = await _cliente.GetAsync($"/senhas/{criada.Id}");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var consultada = await resposta.Content.ReadFromJsonAsync<SenhaRespostaJson>();
        Assert.NotNull(consultada);
        Assert.Equal(criada.Id, consultada.Id);
        Assert.Equal(criada.Senha, consultada.Senha);
        Assert.Equal(criada.CriadaEm, consultada.CriadaEm);
    }

    [Fact]
    public async Task GetSenhas_IdEmFormatoInvalido_Retorna400ComProblemDetails()
    {
        var resposta = await _cliente.GetAsync("/senhas/nao-e-um-guid");

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal(TipoProblemDetails, resposta.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetSenhas_IdInexistente_Retorna404ComProblemDetails()
    {
        var resposta = await _cliente.GetAsync($"/senhas/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Equal(TipoProblemDetails, resposta.Content.Headers.ContentType?.MediaType);
    }
}

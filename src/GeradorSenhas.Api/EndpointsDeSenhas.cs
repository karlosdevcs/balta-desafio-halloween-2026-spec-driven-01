using GeradorSenhas.Application;
using Microsoft.AspNetCore.Mvc;

namespace GeradorSenhas.Api;

public static class EndpointsDeSenhas
{
    public static IEndpointRouteBuilder MapearEndpointsDeSenhas(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/senhas");

        grupo.MapPost("/", async (
                [FromBody] GerarSenhaRequisicao? requisicao,
                SenhaServico servico,
                CancellationToken cancellationToken) =>
            {
                var resposta = await servico.GerarAsync(requisicao?.Tamanho, cancellationToken);
                return Results.Created($"/senhas/{resposta.Id}", resposta);
            })
            .Produces<SenhaResposta>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        // D-08: id recebido como texto para que formato inválido gere 400, e não 404.
        grupo.MapGet("/{id}", async (string id, SenhaServico servico, CancellationToken cancellationToken) =>
                Results.Ok(await servico.ObterAsync(id, cancellationToken)))
            .Produces<SenhaResposta>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}

using GeradorSenhas.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GeradorSenhas.Api;

/// <summary>
/// Traduz exceções da aplicação em ProblemDetails (D-07, D-09).
/// </summary>
internal sealed class TratadorDeExcecoes(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails? problema = exception switch
        {
            ErroDeValidacaoException erro => new ValidationProblemDetails(
                new Dictionary<string, string[]> { [erro.Campo] = [erro.Message] })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Entrada inválida."
            },
            BadHttpRequestException erro => new ProblemDetails
            {
                Status = erro.StatusCode,
                Title = "Requisição inválida.",
                Detail = "O corpo da requisição não pôde ser lido. Verifique se 'tamanho' é um número inteiro."
            },
            SenhaNaoEncontradaException erro => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Senha não encontrada.",
                Detail = erro.Message
            },
            _ => null
        };

        if (problema is null)
            return false;

        httpContext.Response.StatusCode = problema.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problema,
            Exception = exception
        });
    }
}

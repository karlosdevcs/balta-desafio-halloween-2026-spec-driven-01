namespace GeradorSenhas.Application;

/// <summary>
/// Entrada inválida do consumidor (RF-05); traduzida para 400 com ProblemDetails na Api (D-07).
/// </summary>
public sealed class ErroDeValidacaoException(string campo, string mensagem) : Exception(mensagem)
{
    public string Campo { get; } = campo;
}

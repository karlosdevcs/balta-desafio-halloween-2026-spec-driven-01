namespace GeradorSenhas.Application;

/// <summary>
/// Identificador válido sem senha associada (CB-07, CB-08); traduzida para 404 na Api (D-07).
/// </summary>
public sealed class SenhaNaoEncontradaException(Guid id)
    : Exception($"Nenhuma senha encontrada para o identificador '{id}'.")
{
    public Guid Id { get; } = id;
}

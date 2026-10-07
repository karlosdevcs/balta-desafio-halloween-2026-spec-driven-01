namespace GeradorSenhas.Domain;

public sealed class SenhaInvalidaException(IReadOnlyList<string> erros)
    : Exception("A senha não atende à política de senhas: " + string.Join(" ", erros))
{
    public IReadOnlyList<string> Erros { get; } = erros;
}

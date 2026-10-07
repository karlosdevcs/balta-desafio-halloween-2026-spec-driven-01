using GeradorSenhas.Domain;

namespace GeradorSenhas.Application;

public sealed record SenhaResposta(Guid Id, string Senha, DateTimeOffset CriadaEm)
{
    public static SenhaResposta De(Senha senha) => new(senha.Id, senha.Valor, senha.CriadaEm);
}

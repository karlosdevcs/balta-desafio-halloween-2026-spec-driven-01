namespace GeradorSenhas.Domain;

public sealed class Senha
{
    public Guid Id { get; private set; }
    public string Valor { get; private set; } = string.Empty;
    public DateTimeOffset CriadaEm { get; private set; }

    // Usado pelo EF Core na materialização.
    private Senha()
    {
    }

    private Senha(Guid id, string valor, DateTimeOffset criadaEm)
    {
        Id = id;
        Valor = valor;
        CriadaEm = criadaEm;
    }

    /// <summary>
    /// Cria uma senha garantindo a política (D-04) e um identificador gerado pelo sistema (RN-07, D-06).
    /// </summary>
    public static Senha Criar(string valor, DateTimeOffset criadaEm)
    {
        var erros = PoliticaDeSenha.Validar(valor);
        if (erros.Count > 0)
            throw new SenhaInvalidaException(erros);

        var criadaEmEmMicrossegundos = TruncarParaMicrossegundos(criadaEm);
        return new Senha(Guid.CreateVersion7(criadaEmEmMicrossegundos), valor, criadaEmEmMicrossegundos);
    }

    // CA-05: a data devolvida na geração precisa ser idêntica à consultada depois;
    // microssegundo é a maior precisão de data/hora que o armazenamento preserva (D-11).
    private static DateTimeOffset TruncarParaMicrossegundos(DateTimeOffset valor) =>
        valor.AddTicks(-(valor.Ticks % TimeSpan.TicksPerMicrosecond));
}

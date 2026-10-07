namespace GeradorSenhas.Domain;

/// <summary>
/// Regras que toda senha precisa cumprir (RN-01 a RN-05).
/// </summary>
public static class PoliticaDeSenha
{
    public const int TamanhoMinimo = 16;
    public const int TamanhoMaximo = 128;
    public const int TamanhoPadrao = 16;

    public const string LetrasMaiusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string LetrasMinusculas = "abcdefghijklmnopqrstuvwxyz";
    public const string Digitos = "0123456789";
    public const string CaracteresEspeciais = "!@#$%^&*()-_=+[]{};:,.<>?/|~";

    public const string Alfabeto = LetrasMaiusculas + LetrasMinusculas + Digitos + CaracteresEspeciais;

    public static bool TamanhoEhValido(int tamanho) =>
        tamanho is >= TamanhoMinimo and <= TamanhoMaximo;

    /// <summary>
    /// Retorna os motivos pelos quais a senha é inválida; lista vazia significa senha válida.
    /// </summary>
    public static IReadOnlyList<string> Validar(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return ["A senha não pode ser vazia."];

        var erros = new List<string>();

        if (!TamanhoEhValido(valor.Length))
            erros.Add($"A senha deve ter entre {TamanhoMinimo} e {TamanhoMaximo} caracteres.");

        if (valor.Any(char.IsWhiteSpace))
            erros.Add("A senha não pode conter espaços em branco.");

        if (!valor.Any(LetrasMaiusculas.Contains))
            erros.Add("A senha deve conter ao menos uma letra maiúscula.");

        if (!valor.Any(LetrasMinusculas.Contains))
            erros.Add("A senha deve conter ao menos uma letra minúscula.");

        if (!valor.Any(Digitos.Contains))
            erros.Add("A senha deve conter ao menos um dígito.");

        if (!valor.Any(CaracteresEspeciais.Contains))
            erros.Add("A senha deve conter ao menos um caractere especial.");

        if (valor.Any(c => !char.IsWhiteSpace(c) && !Alfabeto.Contains(c)))
            erros.Add("A senha contém caracteres fora do alfabeto permitido.");

        return erros;
    }

    public static bool EhValida(string? valor) => Validar(valor).Count == 0;
}

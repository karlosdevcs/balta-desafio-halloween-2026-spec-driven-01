using System.Security.Cryptography;

namespace GeradorSenhas.Domain;

/// <summary>
/// Gera senhas aleatórias que atendem à <see cref="PoliticaDeSenha"/> (D-02, D-03).
/// </summary>
public static class GeradorDeSenha
{
    private static readonly string[] CategoriasObrigatorias =
    [
        PoliticaDeSenha.LetrasMaiusculas,
        PoliticaDeSenha.LetrasMinusculas,
        PoliticaDeSenha.Digitos,
        PoliticaDeSenha.CaracteresEspeciais
    ];

    public static string Gerar(int tamanho)
    {
        if (!PoliticaDeSenha.TamanhoEhValido(tamanho))
            throw new ArgumentOutOfRangeException(
                nameof(tamanho),
                tamanho,
                $"O tamanho deve estar entre {PoliticaDeSenha.TamanhoMinimo} e {PoliticaDeSenha.TamanhoMaximo}.");

        var caracteres = new char[tamanho];

        // Um caractere de cada categoria garante RN-02 e RN-04 por construção.
        for (var i = 0; i < CategoriasObrigatorias.Length; i++)
            caracteres[i] = Sortear(CategoriasObrigatorias[i]);

        for (var i = CategoriasObrigatorias.Length; i < tamanho; i++)
            caracteres[i] = Sortear(PoliticaDeSenha.Alfabeto);

        Embaralhar(caracteres);
        return new string(caracteres);
    }

    private static char Sortear(string conjunto) =>
        conjunto[RandomNumberGenerator.GetInt32(conjunto.Length)];

    // Fisher-Yates: evita que as categorias obrigatórias fiquem sempre nas primeiras posições (RN-06).
    private static void Embaralhar(char[] caracteres)
    {
        for (var i = caracteres.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (caracteres[i], caracteres[j]) = (caracteres[j], caracteres[i]);
        }
    }
}

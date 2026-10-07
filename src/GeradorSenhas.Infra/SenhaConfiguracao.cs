using GeradorSenhas.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorSenhas.Infra;

internal sealed class SenhaConfiguracao : IEntityTypeConfiguration<Senha>
{
    public void Configure(EntityTypeBuilder<Senha> builder)
    {
        builder.ToTable("senhas");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.Valor)
            .HasColumnName("valor")
            .HasMaxLength(PoliticaDeSenha.TamanhoMaximo)
            .IsRequired();

        builder.Property(s => s.CriadaEm)
            .HasColumnName("criada_em")
            .IsRequired();
    }
}

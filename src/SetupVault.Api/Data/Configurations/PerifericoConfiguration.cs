using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetupVault.Api.Models;

namespace SetupVault.Api.Data.Configurations;

public class PerifericoConfiguration : IEntityTypeConfiguration<Periferico>
{
    public void Configure(EntityTypeBuilder<Periferico> builder)
    {
        builder.ToTable("Perifericos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        // Enum salvo como texto para deixar o banco legível ("Mousepad" em vez de 2)
        builder.Property(p => p.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.Preco)
            .HasPrecision(10, 2);

        builder.Property(p => p.Estoque)
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        builder.HasOne(p => p.Fabricante)
            .WithMany(f => f.Perifericos)
            .HasForeignKey(p => p.FabricanteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

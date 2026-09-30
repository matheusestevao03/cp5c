using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetupVault.Api.Models;

namespace SetupVault.Api.Data.Configurations;

public class FabricanteConfiguration : IEntityTypeConfiguration<Fabricante>
{
    public void Configure(EntityTypeBuilder<Fabricante> builder)
    {
        builder.ToTable("Fabricantes");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.PaisOrigem)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(f => f.SiteOficial)
            .HasMaxLength(200);

        builder.Property(f => f.DataCadastro)
            .IsRequired();

        builder.HasIndex(f => f.Nome)
            .IsUnique();
    }
}

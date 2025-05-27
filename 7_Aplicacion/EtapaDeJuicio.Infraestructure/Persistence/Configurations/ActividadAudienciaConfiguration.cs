using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Infraestructure.Persistence.Configurations;

public class ActividadAudienciaConfiguration : IEntityTypeConfiguration<ActividadAudiencia>
{
    public void Configure(EntityTypeBuilder<ActividadAudiencia> builder)
    {
        builder.ToTable("ActividadesAudiencia");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .IsRequired();

        builder.Property(a => a.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.FechaHora)
            .IsRequired();        builder.Property(a => a.Observaciones)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(a => a.AudienciaId)
            .IsRequired();

        // Propiedades calculadas (ignoradas en la BD)
        builder.Ignore(a => a.Timestamp);

        // Índices
        builder.HasIndex(a => a.Tipo);
        builder.HasIndex(a => a.FechaHora);
        builder.HasIndex(a => a.AudienciaId);
    }
}

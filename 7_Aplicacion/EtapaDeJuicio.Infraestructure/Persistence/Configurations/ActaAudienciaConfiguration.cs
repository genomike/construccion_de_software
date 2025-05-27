using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Infraestructure.Persistence.Configurations;

public class ActaAudienciaConfiguration : IEntityTypeConfiguration<ActaAudiencia>
{
    public void Configure(EntityTypeBuilder<ActaAudiencia> builder)
    {
        builder.ToTable("ActasAudiencia");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .IsRequired();

        builder.Property(a => a.AudienciaId)
            .IsRequired();

        builder.Property(a => a.Contenido)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.FechaGeneracion)
            .IsRequired();

        // Relación con audiencia
        builder.HasOne<Audiencia>()
            .WithMany()
            .HasForeignKey(a => a.AudienciaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(a => a.AudienciaId)
            .IsUnique();
        builder.HasIndex(a => a.FechaGeneracion);
    }
}

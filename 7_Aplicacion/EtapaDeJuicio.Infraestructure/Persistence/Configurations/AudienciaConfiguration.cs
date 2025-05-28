using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Infraestructure.Persistence.Configurations;

public class AudienciaConfiguration : IEntityTypeConfiguration<Audiencia>
{
    public void Configure(EntityTypeBuilder<Audiencia> builder)
    {
        builder.ToTable("Audiencias");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .IsRequired();

        builder.Property(a => a.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.FechaHoraProgramada)
            .IsRequired();

        builder.Property(a => a.Tipo)
            .IsRequired()
            .HasConversion<string>();        builder.Property(a => a.Estado)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.DuracionMinutos)
            .IsRequired()
            .HasDefaultValue(120);

        builder.Property(a => a.FechaHoraInicio)
            .IsRequired(false);

        builder.Property(a => a.FechaHoraFin)
            .IsRequired(false);

        builder.Property(a => a.MotivoCancelacion)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.MotivoSuspension)
            .HasMaxLength(500)
            .IsRequired(false);// Relación con participantes usando backing field
        builder.HasMany(a => a.Participantes)
            .WithOne()
            .HasForeignKey("AudienciaId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Navigation(a => a.Participantes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Relación con actividades usando backing field
        builder.HasMany(a => a.Actividades)
            .WithOne()
            .HasForeignKey("AudienciaId")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Navigation(a => a.Actividades)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Propiedades calculadas (ignoradas en la BD)
        builder.Ignore(a => a.FechaProgramada);
        builder.Ignore(a => a.FechaInicio);
        builder.Ignore(a => a.FechaFin);

        // Índices
        builder.HasIndex(a => a.FechaHoraProgramada);
        builder.HasIndex(a => a.Estado);
        builder.HasIndex(a => a.Tipo);
    }
}

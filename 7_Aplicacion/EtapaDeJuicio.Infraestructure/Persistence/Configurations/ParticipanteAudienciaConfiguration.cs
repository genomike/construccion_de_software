using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Infraestructure.Persistence.Configurations;

public class ParticipanteAudienciaConfiguration : IEntityTypeConfiguration<ParticipanteAudiencia>
{
    public void Configure(EntityTypeBuilder<ParticipanteAudiencia> builder)
    {
        builder.ToTable("ParticipantesAudiencia");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Rol)
            .IsRequired()
            .HasConversion<string>();        builder.Property(p => p.FechaRegistro)
            .IsRequired();

        builder.Property(p => p.AudienciaId)
            .IsRequired();

        // Índices
        builder.HasIndex(p => p.Rol);
        builder.HasIndex(p => p.AudienciaId);
    }
}

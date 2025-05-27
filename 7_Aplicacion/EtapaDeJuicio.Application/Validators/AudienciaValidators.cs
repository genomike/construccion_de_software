using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using FluentValidation;

namespace EtapaDeJuicio.Application.Validators;

public class CrearAudienciaCommandValidator : AbstractValidator<CrearAudienciaCommand>
{
    public CrearAudienciaCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El título de la audiencia es obligatorio")
            .MaximumLength(200)
            .WithMessage("El título no puede exceder los 200 caracteres");

        RuleFor(x => x.FechaProgramada)
            .GreaterThan(DateTime.Now)
            .WithMessage("La fecha programada debe ser futura");

        RuleFor(x => x.TipoAudiencia)
            .Must(BeAValidTipoAudiencia)
            .WithMessage("El tipo de audiencia no es válido");

        RuleFor(x => x.DuracionMinutos)
            .GreaterThan(0)
            .WithMessage("La duración debe ser mayor a 0 minutos")
            .LessThanOrEqualTo(480)
            .WithMessage("La duración no puede exceder 8 horas");
    }

    private static bool BeAValidTipoAudiencia(int tipoAudiencia)
    {
        return Enum.IsDefined(typeof(TipoAudiencia), tipoAudiencia);
    }
}

public class IniciarAudienciaCommandValidator : AbstractValidator<IniciarAudienciaCommand>
{
    public IniciarAudienciaCommandValidator()
    {
        RuleFor(x => x.AudienciaId)
            .NotEmpty()
            .WithMessage("El ID de la audiencia es obligatorio");
    }
}

public class AgregarParticipanteCommandValidator : AbstractValidator<AgregarParticipanteCommand>
{
    public AgregarParticipanteCommandValidator()
    {
        RuleFor(x => x.AudienciaId)
            .NotEmpty()
            .WithMessage("El ID de la audiencia es obligatorio");

        RuleFor(x => x.ParticipanteId)
            .NotEmpty()
            .WithMessage("El ID del participante es obligatorio");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del participante es obligatorio")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder los 100 caracteres");

        RuleFor(x => x.RolParticipante)
            .IsInEnum()
            .WithMessage("El rol del participante no es válido");
    }
}

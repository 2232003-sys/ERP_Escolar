using FluentValidation;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Validators;

/// <summary>
/// Validador para crear inscripción.
/// </summary>
public class CreateInscripcionValidator : AbstractValidator<CreateInscripcionDto>
{
    public CreateInscripcionValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0)
            .WithMessage("El alumno es requerido.");

        RuleFor(x => x.GrupoId)
            .GreaterThan(0)
            .WithMessage("El grupo es requerido.");

        RuleFor(x => x.CicloEscolarId)
            .GreaterThan(0)
            .WithMessage("El ciclo escolar es requerido.");

        RuleFor(x => x.FechaInscripcion)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.FechaInscripcion.HasValue)
            .WithMessage("La fecha de inscripción no puede ser futura.");
    }
}

/// <summary>
/// Validador para actualizar inscripción.
/// </summary>
public class UpdateInscripcionValidator : AbstractValidator<UpdateInscripcionDto>
{
    public UpdateInscripcionValidator()
    {
        RuleFor(x => x.GrupoId)
            .GreaterThan(0)
            .When(x => x.GrupoId.HasValue)
            .WithMessage("El ID del grupo debe ser válido.");

        RuleFor(x => x.FechaInscripcion)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.FechaInscripcion.HasValue)
            .WithMessage("La fecha de inscripción no puede ser futura.");
    }
}

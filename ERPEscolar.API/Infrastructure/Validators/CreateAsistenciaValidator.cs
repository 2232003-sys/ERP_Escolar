using FluentValidation;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Validators;

/// <summary>
/// Validador para crear asistencias
/// </summary>
public class CreateAsistenciaValidator : AbstractValidator<CreateAsistenciaDto>
{
    public CreateAsistenciaValidator()
    {
        RuleFor(x => x.InscripcionId)
            .GreaterThan(0)
            .WithMessage("La inscripción debe ser válida");

        RuleFor(x => x.GrupoMateriaId)
            .GreaterThan(0)
            .WithMessage("La materia debe ser válida");

        RuleFor(x => x.Fecha)
            .NotEmpty()
            .WithMessage("La fecha es requerida")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("La fecha no puede ser futura");

        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("El estado de asistencia es requerido")
            .Must(estado => new[] { "Presente", "Ausente", "Justificado", "Retraso" }.Contains(estado))
            .WithMessage("El estado debe ser: Presente, Ausente, Justificado o Retraso");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden exceder 500 caracteres");
    }
}

/// <summary>
/// Validador para actualizar asistencias
/// </summary>
public class UpdateAsistenciaValidator : AbstractValidator<UpdateAsistenciaDto>
{
    public UpdateAsistenciaValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("El estado de asistencia es requerido")
            .Must(estado => new[] { "Presente", "Ausente", "Justificado", "Retraso" }.Contains(estado))
            .WithMessage("El estado debe ser: Presente, Ausente, Justificado o Retraso");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden exceder 500 caracteres");
    }
}

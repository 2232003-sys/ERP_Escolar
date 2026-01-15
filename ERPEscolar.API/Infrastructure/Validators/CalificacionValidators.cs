using ERPEscolar.API.DTOs.ControlEscolar;
using FluentValidation;

namespace ERPEscolar.API.Validators;

/// <summary>
/// Validador para CreateCalificacionDto
/// </summary>
public class CreateCalificacionValidator : AbstractValidator<CreateCalificacionDto>
{
    public CreateCalificacionValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0)
            .WithMessage("El ID del alumno debe ser mayor a 0");

        RuleFor(x => x.GrupoMateriaId)
            .GreaterThan(0)
            .WithMessage("El ID del grupo-materia debe ser mayor a 0");

        RuleFor(x => x.PeriodoCalificacionId)
            .GreaterThan(0)
            .WithMessage("El ID del período de calificación debe ser mayor a 0");

        RuleFor(x => x.Calificacion1)
            .InclusiveBetween(0, 10)
            .WithMessage("La calificación debe estar entre 0 y 10");

        RuleFor(x => x.Observacion)
            .MaximumLength(500)
            .WithMessage("La observación no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacion));

        RuleFor(x => x.DocenteQueQualificaId)
            .GreaterThan(0)
            .WithMessage("El ID del docente debe ser mayor a 0")
            .When(x => x.DocenteQueQualificaId.HasValue);
    }
}

/// <summary>
/// Validador para UpdateCalificacionDto
/// </summary>
public class UpdateCalificacionValidator : AbstractValidator<UpdateCalificacionDto>
{
    public UpdateCalificacionValidator()
    {
        RuleFor(x => x.Calificacion1)
            .InclusiveBetween(0, 10)
            .WithMessage("La calificación debe estar entre 0 y 10");

        RuleFor(x => x.Observacion)
            .MaximumLength(500)
            .WithMessage("La observación no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacion));
    }
}
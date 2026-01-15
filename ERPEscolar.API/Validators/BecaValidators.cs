using ERPEscolar.API.DTOs.Finanzas;
using FluentValidation;

namespace ERPEscolar.API.Validators;

public class CreateBecaValidator : AbstractValidator<CreateBecaDto>
{
    public CreateBecaValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0).WithMessage("El ID del alumno debe ser mayor a 0");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la beca es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El tipo de beca es requerido")
            .MaximumLength(20).WithMessage("El tipo no puede exceder 20 caracteres")
            .Must(BeValidTipo).WithMessage("Tipo de beca inválido");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("El valor debe ser mayor a 0");

        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio")
            .When(x => x.FechaFin.HasValue);

        RuleFor(x => x.Justificacion)
            .MaximumLength(500).WithMessage("La justificación no puede exceder 500 caracteres");
    }

    private bool BeValidTipo(string tipo)
    {
        var validTipos = new[] { "Porcentaje", "Fijo" };
        return validTipos.Contains(tipo);
    }
}

public class UpdateBecaValidator : AbstractValidator<UpdateBecaDto>
{
    public UpdateBecaValidator()
    {
        RuleFor(x => x.Nombre)
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Nombre));

        RuleFor(x => x.Tipo)
            .MaximumLength(20).WithMessage("El tipo no puede exceder 20 caracteres")
            .Must(BeValidTipo).WithMessage("Tipo de beca inválido")
            .When(x => !string.IsNullOrEmpty(x.Tipo));

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("El valor debe ser mayor a 0")
            .When(x => x.Valor.HasValue);

        RuleFor(x => x.Justificacion)
            .MaximumLength(500).WithMessage("La justificación no puede exceder 500 caracteres");
    }

    private bool BeValidTipo(string tipo)
    {
        var validTipos = new[] { "Porcentaje", "Fijo" };
        return validTipos.Contains(tipo);
    }
}
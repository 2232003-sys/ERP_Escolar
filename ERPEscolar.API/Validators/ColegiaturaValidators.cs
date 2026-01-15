using ERPEscolar.API.DTOs.Finanzas;
using FluentValidation;

namespace ERPEscolar.API.Validators;

/// <summary>
/// Validador para la creación de colegiaturas
/// </summary>
public class CreateColegiaturaValidator : AbstractValidator<CreateColegiaturaDto>
{
    public CreateColegiaturaValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0)
            .WithMessage("El ID del alumno debe ser mayor a 0");

        RuleFor(x => x.ConceptoCobroId)
            .GreaterThan(0)
            .WithMessage("El ID del concepto de cobro debe ser mayor a 0");

        RuleFor(x => x.CicloEscolarId)
            .GreaterThan(0)
            .WithMessage("El ID del ciclo escolar debe ser mayor a 0");

        RuleFor(x => x.Mes)
            .NotEmpty()
            .WithMessage("El mes es requerido")
            .Matches(@"^\d{4}-\d{2}$")
            .WithMessage("El mes debe tener el formato YYYY-MM")
            .Must(BeValidMonth)
            .WithMessage("El mes debe ser válido (01-12)");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("El monto no puede exceder 999,999.99");

        RuleFor(x => x.Descuento)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El descuento no puede ser negativo")
            .LessThanOrEqualTo(x => x.Monto)
            .WithMessage("El descuento no puede ser mayor al monto");

        RuleFor(x => x.Recargo)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El recargo no puede ser negativo");

        RuleFor(x => x.IVA)
            .InclusiveBetween(0, 1)
            .WithMessage("El IVA debe estar entre 0 y 1");

        RuleFor(x => x.FechaVencimiento)
            .GreaterThan(DateTime.Now)
            .When(x => x.FechaVencimiento.HasValue)
            .WithMessage("La fecha de vencimiento debe ser futura");

        RuleFor(x => x.Observacion)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden exceder 500 caracteres");
    }

    private bool BeValidMonth(string mes)
    {
        if (string.IsNullOrEmpty(mes) || mes.Length != 7) return false;

        var parts = mes.Split('-');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
            return false;

        return month >= 1 && month <= 12 && year >= 2000 && year <= 2100;
    }
}

/// <summary>
/// Validador para la actualización de colegiaturas
/// </summary>
public class UpdateColegiaturaValidator : AbstractValidator<UpdateColegiaturaDto>
{
    public UpdateColegiaturaValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("El estado es requerido")
            .MaximumLength(20)
            .WithMessage("El estado no puede exceder 20 caracteres")
            .Must(BeValidEstado)
            .WithMessage("El estado debe ser: Pendiente, Parcial, Pagado o Cancelado");

        RuleFor(x => x.MontoRecibido)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El monto recibido no puede ser negativo");

        RuleFor(x => x.FechaPago)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.FechaPago.HasValue)
            .WithMessage("La fecha de pago no puede ser futura");

        RuleFor(x => x.Observacion)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden exceder 500 caracteres");
    }

    private bool BeValidEstado(string estado)
    {
        var estadosValidos = new[] { "Pendiente", "Parcial", "Pagado", "Cancelado" };
        return estadosValidos.Contains(estado);
    }
}
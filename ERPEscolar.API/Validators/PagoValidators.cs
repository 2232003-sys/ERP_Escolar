using ERPEscolar.API.DTOs.Finanzas;
using FluentValidation;

namespace ERPEscolar.API.Validators;

public class CreatePagoValidator : AbstractValidator<CreatePagoDto>
{
    public CreatePagoValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0).WithMessage("El ID del alumno debe ser mayor a 0");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.Metodo)
            .NotEmpty().WithMessage("El método de pago es requerido")
            .MaximumLength(50).WithMessage("El método de pago no puede exceder 50 caracteres")
            .Must(BeValidMetodo).WithMessage("Método de pago inválido");

        RuleFor(x => x.ReferenciaExterna)
            .MaximumLength(100).WithMessage("La referencia externa no puede exceder 100 caracteres");

        RuleFor(x => x.BancoOrigen)
            .MaximumLength(100).WithMessage("El banco origen no puede exceder 100 caracteres");

        RuleFor(x => x.Observacion)
            .MaximumLength(500).WithMessage("La observación no puede exceder 500 caracteres");

        RuleFor(x => x.FechaPago)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("La fecha de pago no puede ser futura");
    }

    private bool BeValidMetodo(string metodo)
    {
        var validMetodos = new[] { "Efectivo", "Transferencia", "Tarjeta", "Cheque", "Depósito" };
        return validMetodos.Contains(metodo);
    }
}

public class UpdatePagoValidator : AbstractValidator<UpdatePagoDto>
{
    public UpdatePagoValidator()
    {
        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0")
            .When(x => x.Monto.HasValue);

        RuleFor(x => x.Metodo)
            .MaximumLength(50).WithMessage("El método de pago no puede exceder 50 caracteres")
            .Must(BeValidMetodo).WithMessage("Método de pago inválido")
            .When(x => !string.IsNullOrEmpty(x.Metodo));

        RuleFor(x => x.ReferenciaExterna)
            .MaximumLength(100).WithMessage("La referencia externa no puede exceder 100 caracteres");

        RuleFor(x => x.BancoOrigen)
            .MaximumLength(100).WithMessage("El banco origen no puede exceder 100 caracteres");

        RuleFor(x => x.Observacion)
            .MaximumLength(500).WithMessage("La observación no puede exceder 500 caracteres");

        RuleFor(x => x.FechaPago)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("La fecha de pago no puede ser futura")
            .When(x => x.FechaPago.HasValue);
    }

    private bool BeValidMetodo(string metodo)
    {
        var validMetodos = new[] { "Efectivo", "Transferencia", "Tarjeta", "Cheque", "Depósito" };
        return validMetodos.Contains(metodo);
    }
}

public class PagoTransferenciaValidator : AbstractValidator<PagoTransferenciaDto>
{
    public PagoTransferenciaValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0).WithMessage("El ID del alumno debe ser mayor a 0");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.ReferenciaExterna)
            .NotEmpty().WithMessage("La referencia externa es requerida para transferencias")
            .MaximumLength(100).WithMessage("La referencia externa no puede exceder 100 caracteres");

        RuleFor(x => x.BancoOrigen)
            .MaximumLength(100).WithMessage("El banco origen no puede exceder 100 caracteres");

        RuleFor(x => x.Observacion)
            .MaximumLength(500).WithMessage("La observación no puede exceder 500 caracteres");
    }
}
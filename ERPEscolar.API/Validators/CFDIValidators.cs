using ERPEscolar.API.DTOs.Fiscal;
using FluentValidation;

namespace ERPEscolar.API.Validators;

/// <summary>
/// Validador para la creación de CFDI
/// </summary>
public class CreateCFDIValidator : AbstractValidator<CreateCFDIDto>
{
    public CreateCFDIValidator()
    {
        RuleFor(x => x.CargoId)
            .GreaterThan(0)
            .WithMessage("El ID del cargo debe ser mayor a 0");

        RuleFor(x => x.RFC_Receptor)
            .NotEmpty()
            .WithMessage("El RFC del receptor es requerido")
            .Matches(@"^[A-ZÑ&]{3,4}[0-9]{6}[A-Z0-9]{3}$")
            .WithMessage("El RFC del receptor no tiene un formato válido");

        RuleFor(x => x.NombreReceptor)
            .NotEmpty()
            .WithMessage("El nombre del receptor es requerido")
            .MaximumLength(254)
            .WithMessage("El nombre del receptor no puede exceder 254 caracteres");

        RuleFor(x => x.NivelEducativo)
            .NotEmpty()
            .WithMessage("El nivel educativo es requerido")
            .MaximumLength(50)
            .WithMessage("El nivel educativo no puede exceder 50 caracteres")
            .Must(BeValidNivelEducativo)
            .WithMessage("El nivel educativo debe ser: Preescolar, Primaria, Secundaria, Bachillerato, Profesional o Postgrado");

        RuleFor(x => x.CURP_Alumno)
            .NotEmpty()
            .WithMessage("La CURP del alumno es requerida")
            .Matches(@"^[A-Z]{1}[AEIOU]{1}[A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])[HM]{1}(AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|TM|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z]{1}[0-9]{1}$")
            .WithMessage("La CURP del alumno no tiene un formato válido");

        RuleFor(x => x.ClaveCT)
            .NotEmpty()
            .WithMessage("La clave CT es requerida")
            .MaximumLength(10)
            .WithMessage("La clave CT no puede exceder 10 caracteres");

        RuleFor(x => x.Serie)
            .MaximumLength(25)
            .WithMessage("La serie no puede exceder 25 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Serie));

        RuleFor(x => x.Folio)
            .MaximumLength(40)
            .WithMessage("El folio no puede exceder 40 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Folio));
    }

    private bool BeValidNivelEducativo(string nivel)
    {
        var nivelesValidos = new[] {
            "Preescolar", "Primaria", "Secundaria",
            "Bachillerato", "Profesional", "Postgrado"
        };
        return nivelesValidos.Contains(nivel);
    }
}

/// <summary>
/// Validador para la actualización de CFDI
/// </summary>
public class UpdateCFDIValidator : AbstractValidator<UpdateCFDIDto>
{
    public UpdateCFDIValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("El estado es requerido")
            .MaximumLength(20)
            .WithMessage("El estado no puede exceder 20 caracteres")
            .Must(BeValidEstado)
            .WithMessage("El estado debe ser: Borrador, Timbrada, Cancelada o Error");

        RuleFor(x => x.FechaCancelacion)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.FechaCancelacion.HasValue)
            .WithMessage("La fecha de cancelación no puede ser futura");

        RuleFor(x => x.RazonCancelacion)
            .NotEmpty()
            .When(x => x.Estado == "Cancelada")
            .WithMessage("La razón de cancelación es requerida cuando el estado es Cancelada")
            .MaximumLength(500)
            .WithMessage("La razón de cancelación no puede exceder 500 caracteres");

        RuleFor(x => x.ErrorTimbrado)
            .MaximumLength(1000)
            .WithMessage("El error de timbrado no puede exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ErrorTimbrado));
    }

    private bool BeValidEstado(string estado)
    {
        var estadosValidos = new[] { "Borrador", "Timbrada", "Cancelada", "Error" };
        return estadosValidos.Contains(estado);
    }
}

/// <summary>
/// Validador para timbrado de CFDI
/// </summary>
public class TimbrarCFDIValidator : AbstractValidator<TimbrarCFDIDto>
{
    public TimbrarCFDIValidator()
    {
        RuleFor(x => x.CFDIId)
            .GreaterThan(0)
            .WithMessage("El ID del CFDI debe ser mayor a 0");
    }
}

/// <summary>
/// Validador para cancelación de CFDI
/// </summary>
public class CancelarCFDIValidator : AbstractValidator<CancelarCFDIDto>
{
    public CancelarCFDIValidator()
    {
        RuleFor(x => x.CFDIId)
            .GreaterThan(0)
            .WithMessage("El ID del CFDI debe ser mayor a 0");

        RuleFor(x => x.RazonCancelacion)
            .NotEmpty()
            .WithMessage("La razón de cancelación es requerida")
            .MaximumLength(500)
            .WithMessage("La razón de cancelación no puede exceder 500 caracteres");
    }
}

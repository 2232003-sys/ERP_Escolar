using FluentValidation;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Validators;

/// <summary>
/// Validador para CreateAlumnoDto usando FluentValidation.
/// Proporciona validaciones complejas con mensajes claros en español.
/// </summary>
public class CreateAlumnoValidator : AbstractValidator<CreateAlumnoDto>
{
    public CreateAlumnoValidator()
    {
        // Validación de Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del alumno es obligatorio.")
            .MinimumLength(2)
            .WithMessage("El nombre debe tener al menos 2 caracteres.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s'-]+$")
            .WithMessage("El nombre solo puede contener letras, espacios, guiones y apóstrofes.");

        // Validación de Apellido
        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("El apellido del alumno es obligatorio.")
            .MinimumLength(2)
            .WithMessage("El apellido debe tener al menos 2 caracteres.")
            .MaximumLength(100)
            .WithMessage("El apellido no puede exceder 100 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s'-]+$")
            .WithMessage("El apellido solo puede contener letras, espacios, guiones y apóstrofes.");

        // Validación de CURP
        RuleFor(x => x.CURP)
            .NotEmpty()
            .WithMessage("El CURP del alumno es obligatorio.")
            .Length(18)
            .WithMessage("El CURP debe tener exactamente 18 caracteres.")
            .Matches(@"^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9]{2}$")
            .WithMessage("El formato del CURP no es válido. Debe seguir el patrón: XXXXXX000000HXXXXX00");

        // Validación de Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email del alumno es obligatorio.")
            .EmailAddress()
            .WithMessage("El email no tiene un formato válido.")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder 255 caracteres.");

        // Validación de Fecha de Nacimiento
        RuleFor(x => x.FechaNacimiento)
            .NotEmpty()
            .WithMessage("La fecha de nacimiento del alumno es obligatoria.")
            .LessThan(DateTime.Today)
            .WithMessage("La fecha de nacimiento no puede ser en el futuro.")
            .GreaterThan(DateTime.Today.AddYears(-100))
            .WithMessage("La fecha de nacimiento no puede ser hace más de 100 años.")
            .Custom((fechaNacimiento, context) =>
            {
                var edad = DateTime.Today.Year - fechaNacimiento.Year;
                if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad))
                    edad--;

                if (edad < 3)
                {
                    context.AddFailure(nameof(fechaNacimiento), 
                        "El alumno debe tener al menos 3 años de edad.");
                }

                if (edad > 25)
                {
                    context.AddFailure(nameof(fechaNacimiento), 
                        "La edad del alumno parece ser muy alta para estar en escuela primaria/secundaria. Verifica la fecha de nacimiento.");
                }
            });

        // Validación de Género
        RuleFor(x => x.Sexo)
            .NotEmpty()
            .WithMessage("El sexo del alumno es obligatorio.")
            .Must(x => x == "M" || x == "F")
            .WithMessage("El sexo debe ser 'M' (Masculino) o 'F' (Femenino).");

        // Validación de Dirección (opcional pero si viene, validar)
        RuleFor(x => x.Direccion)
            .MaximumLength(500)
            .WithMessage("La dirección no puede exceder 500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Direccion));

        // Validación de Teléfono de Contacto (opcional pero si viene, validar)
        RuleFor(x => x.TelefonoContacto)
            .Matches(@"^[0-9+\-\s()]+$")
            .WithMessage("El formato del teléfono no es válido. Solo se permiten números, +, -, espacios y paréntesis.")
            .MaximumLength(20)
            .WithMessage("El teléfono no puede exceder 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.TelefonoContacto));

        // Validación de TutorId (opcional pero si viene, debe ser > 0)
        RuleFor(x => x.TutorId)
            .GreaterThan(0)
            .WithMessage("El ID del tutor debe ser válido.")
            .When(x => x.TutorId.HasValue);
    }
}

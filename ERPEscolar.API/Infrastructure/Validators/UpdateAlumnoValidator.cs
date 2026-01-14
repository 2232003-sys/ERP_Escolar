using FluentValidation;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Validators;

/// <summary>
/// Validador para UpdateAlumnoDto usando FluentValidation.
/// Proporciona validaciones para actualización de alumnos.
/// </summary>
public class UpdateAlumnoValidator : AbstractValidator<UpdateAlumnoDto>
{
    public UpdateAlumnoValidator()
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

        // Validación de Sexo
        RuleFor(x => x.Sexo)
            .NotEmpty()
            .WithMessage("El sexo del alumno es obligatorio.")
            .Must(x => x == "M" || x == "F")
            .WithMessage("El sexo debe ser 'M' (Masculino) o 'F' (Femenino).");
    }
}

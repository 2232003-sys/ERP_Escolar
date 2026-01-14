using FluentValidation;
using ERPEscolar.API.DTOs.ControlEscolar;

namespace ERPEscolar.API.Infrastructure.Validators;

/// <summary>
/// Validador para CreateGrupoDto usando FluentValidation.
/// Valida campos requeridos, longitudes, y valores de negocio.
/// </summary>
public class CreateGrupoValidator : AbstractValidator<CreateGrupoDto>
{
    public CreateGrupoValidator()
    {
        // Validación de SchoolId
        RuleFor(x => x.SchoolId)
            .GreaterThan(0)
            .WithMessage("El SchoolId debe ser un valor positivo.");

        // Validación de CicloEscolarId
        RuleFor(x => x.CicloEscolarId)
            .GreaterThan(0)
            .WithMessage("El CicloEscolarId debe ser un valor positivo.");

        // Validación de Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del grupo es obligatorio.")
            .MinimumLength(1)
            .WithMessage("El nombre debe tener al menos 1 carácter.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres.");

        // Validación de Grado
        //RuleFor(x => x.Grado)
          //  .NotEmpty()
            //.WithMessage("El grado del grupo es obligatorio.")
            //.MinimumLength(1)
            //.WithMessage("El grado debe tener al menos 1 carácter.")
            //.MaximumLength(50)
            //.WithMessage("El grado no puede exceder 50 caracteres.");
        // Validación de Seccion
       // RuleFor(x => x.Seccion)
         //   .NotEmpty()
           // .WithMessage("La sección del grupo es obligatoria.")
            //.MinimumLength(1)
            //.WithMessage("La sección debe tener al menos 1 carácter.")
            //.MaximumLength(10)
            //.WithMessage("La sección no puede exceder 10 caracteres.");
        // Validación de CapacidadMaxima
        RuleFor(x => x.CapacidadMaxima)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La capacidad máxima debe ser al menos 1.")
            .LessThanOrEqualTo(200)
            .WithMessage("La capacidad máxima no puede exceder 200.");

        // Validación de DocenteTutorId (opcional)
        RuleFor(x => x.DocenteTutorId)
            .GreaterThan(0)
            .WithMessage("El DocenteTutorId debe ser un valor positivo.")
            .When(x => x.DocenteTutorId.HasValue);
    }
}

/// <summary>
/// Validador para UpdateGrupoDto usando FluentValidation.
/// Valida campos permitidos para actualizar.
/// </summary>
public class UpdateGrupoValidator : AbstractValidator<UpdateGrupoDto>
{
    public UpdateGrupoValidator()
    {
        // Validación de Nombre
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del grupo es obligatorio.")
            .MinimumLength(1)
            .WithMessage("El nombre debe tener al menos 1 carácter.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres.");

        // Validación de Grado
       // RuleFor(x => x.Grado)
         //   .NotEmpty()
           // .WithMessage("El grado del grupo es obligatorio.")
            //.MinimumLength(1)
            //.WithMessage("El grado debe tener al menos 1 carácter.")
            //.MaximumLength(50)
            //.WithMessage("El grado no puede exceder 50 caracteres.");

        // Validación de Seccion
        //RuleFor(x => x.Seccion)
          //  .NotEmpty()
            //.WithMessage("La sección del grupo es obligatoria.")
            //.MinimumLength(1)
            //.WithMessage("La sección debe tener al menos 1 carácter.")
            //.MaximumLength(50)
            //.WithMessage("La sección no puede exceder 50 caracteres.");

        // Validación de CapacidadMaxima
        RuleFor(x => x.CapacidadMaxima)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La capacidad máxima debe ser al menos 1.")
            .LessThanOrEqualTo(200)
            .WithMessage("La capacidad máxima no puede exceder 200.");

        // Validación de DocenteTutorId (opcional)
        RuleFor(x => x.DocenteTutorId)
            .GreaterThan(0)
            .WithMessage("El DocenteTutorId debe ser un valor positivo.")
            .When(x => x.DocenteTutorId.HasValue);
    }
}

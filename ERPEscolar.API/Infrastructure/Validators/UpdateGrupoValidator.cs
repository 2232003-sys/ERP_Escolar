
using ERPEscolar.DTOs.ControlEscolar;
using FluentValidation;

namespace ERPEscolar.API.Infrastructure.Validators
{
    public class UpdateGrupoValidator : AbstractValidator<UpdateGrupoDto>
    {
        public UpdateGrupoValidator()
        {
            RuleFor(g => g.Nombre)
                .NotEmpty().WithMessage("El nombre del grupo es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(g => g.Grado)
                .InclusiveBetween(1, 12).WithMessage("El grado debe estar entre 1 y 12.");

            RuleFor(g => g.Turno)
                .NotEmpty().WithMessage("El turno es obligatorio.")
                .Must(t => new[] { "Matutino", "Vespertino", "Nocturno" }.Contains(t))
                .WithMessage("El turno debe ser 'Matutino', 'Vespertino' o 'Nocturno'.");

            RuleFor(g => g.CapacidadMaxima)
                .GreaterThan(0).WithMessage("La capacidad máxima debe ser mayor a 0.");

            RuleFor(g => g.CicloEscolarId)
                .NotEmpty().WithMessage("El ID del ciclo escolar es obligatorio.");
        }
    }
}

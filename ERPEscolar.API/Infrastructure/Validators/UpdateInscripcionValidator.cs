
using ERPEscolar.DTOs.ControlEscolar;
using FluentValidation;

namespace ERPEscolar.API.Infrastructure.Validators
{
    public class UpdateInscripcionValidator : AbstractValidator<UpdateInscripcionDto>
    {
        public UpdateInscripcionValidator()
        {
            RuleFor(i => i.AlumnoId).NotEmpty().WithMessage("El ID del alumno es obligatorio.");
            RuleFor(i => i.GrupoId).NotEmpty().WithMessage("El ID del grupo es obligatorio.");
            RuleFor(i => i.FechaInscripcion).NotEmpty().WithMessage("La fecha de inscripción es obligatoria.");
        }
    }
}


using System;
using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.DTOs.ControlEscolar
{
    public class CreateInscripcionDto
    {
        [Required]
        public int AlumnoId { get; set; }

        [Required]
        public int GrupoId { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    }
}

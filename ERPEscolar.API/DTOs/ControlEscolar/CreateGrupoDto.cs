
using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.DTOs.ControlEscolar
{
    public class CreateGrupoDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Range(1, 20)]
        public int Grado { get; set; }

        [Required]
        public string Turno { get; set; }

        [Range(1, 100)]
        public int CapacidadMaxima { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public int CicloEscolarId { get; set; }
    }
}

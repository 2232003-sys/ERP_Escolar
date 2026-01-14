namespace ERPEscolar.API.Models;

/// <summary>
/// Representa una institución educativa (plantel/campus).
/// Base para futuro multi-plantel.
/// </summary>
public class School
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? RFC { get; set; }
    public string? ClaveCT { get; set; } // Clave de Centro de Trabajo (SEP)
    public string Direccion { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    // Relaciones
    public ICollection<CicloEscolar> CiclosEscolares { get; set; } = [];
    public ICollection<Alumno> Alumnos { get; set; } = [];
    public ICollection<Docente> Docentes { get; set; } = [];
    public ICollection<Grupo> Grupos { get; set; } = [];
    public ICollection<ConceptoCobro> ConceptosCobro { get; set; } = [];
}

namespace ERPEscolar.API.Models;

/// <summary>
/// Tutor/Representante legal de alumno (padre, madre, apoderado).
/// </summary>
public class Tutor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public string? RFC { get; set; }
    public string? CURP { get; set; }
    public string Parentesco { get; set; } = null!; // Padre, Madre, Abuelo, etc.
    public bool PrincipalResponsable { get; set; } = false;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    public ICollection<Alumno> Alumnos { get; set; } = [];
}

/// <summary>
/// Estudiante/Alumno.
/// </summary>
public class Alumno
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CURP { get; set; } = null!;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!; // M, F
    public string? Matricula { get; set; } // Código único del alumno
    public bool Activo { get; set; } = true;
    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    // Relaciones
    public School School { get; set; } = null!;
    public ICollection<Tutor> Tutores { get; set; } = [];
    public ICollection<Inscripcion> Inscripciones { get; set; } = [];
    public ICollection<Asistencia> Asistencias { get; set; } = [];
    public ICollection<Calificacion> Calificaciones { get; set; } = [];
    public ICollection<Cargo> Cargos { get; set; } = [];
    public ICollection<Pago> Pagos { get; set; } = [];
}

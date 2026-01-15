namespace ERPEscolar.API.Models;

/// <summary>
/// Docente/Profesor.
/// </summary>
public class Docente
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int UserId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? RFC { get; set; }
    public string? CURP { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaContratacion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    public School School { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Materia> Materias { get; set; } = [];
    public ICollection<Grupo> Grupos { get; set; } = [];
    public ICollection<Asistencia> AsistenciasRegistradas { get; set; } = [];
}

/// <summary>
/// Materia/Asignatura/Curso.
/// </summary>
public class Materia
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string Nombre { get; set; } = null!; // Matemáticas, Español, etc.
    public string Clave { get; set; } = null!; // MAT-01
    public string? Descripcion { get; set; }
    public decimal Creditos { get; set; } = 1;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public School School { get; set; } = null!;
    public ICollection<GrupoMateria> GrupoMaterias { get; set; } = [];
}

/// <summary>
/// Grupo/Clase (ej: 1ro A, 3ro B).
/// </summary>
public class Grupo
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int CicloEscolarId { get; set; }
    public string Nombre { get; set; } = null!; // "1ro A"
    public string Grado { get; set; } = null!; // "1ro", "2do", etc.
    public string Seccion { get; set; } = null!; // "A", "B", etc.
    public int? DocenteTutorId { get; set; }
    public int CapacidadMaxima { get; set; } = 35;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public School School { get; set; } = null!;
    public CicloEscolar CicloEscolar { get; set; } = null!;
    public Docente? DocenteTutor { get; set; }
    public ICollection<Inscripcion> Inscripciones { get; set; } = [];
    public ICollection<GrupoMateria> GrupoMaterias { get; set; } = [];
}

/// <summary>
/// Relación entre Grupo y Materia (asignación de materias a grupos).
/// </summary>
public class GrupoMateria
{
    public int Id { get; set; }
    public int GrupoId { get; set; }
    public int MateriaId { get; set; }
    public int DocenteId { get; set; }
    public decimal Peso { get; set; } = 1; // Para promedios ponderados
    public bool Activo { get; set; } = true;

    public Grupo Grupo { get; set; } = null!;
    public Materia Materia { get; set; } = null!;
    public Docente Docente { get; set; } = null!;
    public ICollection<Calificacion> Calificaciones { get; set; } = [];
    public ICollection<Asistencia> Asistencias { get; set; } = [];
}

/// <summary>
/// Inscripción de alumno a grupo en ciclo escolar.
/// </summary>
public class Inscripcion
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int GrupoId { get; set; }
    public int CicloEscolarId { get; set; }
    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Alumno Alumno { get; set; } = null!;
    public Grupo Grupo { get; set; } = null!;
    public CicloEscolar CicloEscolar { get; set; } = null!;
    public ICollection<Asistencia> Asistencias { get; set; } = [];
    public ICollection<Calificacion> Calificaciones { get; set; } = [];
}

/// <summary>
/// Registro de asistencia (alumno a clase).
/// </summary>
public class Asistencia
{
    public int Id { get; set; }
    public int InscripcionId { get; set; }
    public int GrupoMateriaId { get; set; }
    public int? DocenteQueRegistroId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = null!; // "Presente", "Ausente", "Tarde", "Justificado"
    public string? Observaciones { get; set; }
    public string? RegistradoPor { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Inscripcion Inscripcion { get; set; } = null!;
    public GrupoMateria GrupoMateria { get; set; } = null!;
    public Docente? DocenteQueRegistro { get; set; }
}

/// <summary>
/// Calificación de alumno en una materia por período.
/// </summary>
public class Calificacion
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public int GrupoMateriaId { get; set; }
    public int PeriodoCalificacionId { get; set; }
    public decimal Calificacion1 { get; set; } // Puede ser promedio, nota, etc.
    public string? Observacion { get; set; }
    public bool Aprobado { get; set; }
    public DateTime FechaCalificacion { get; set; } = DateTime.UtcNow;
    public int? DocenteQueQualificaId { get; set; }

    public Alumno Alumno { get; set; } = null!;
    public GrupoMateria GrupoMateria { get; set; } = null!;
    public PeriodoCalificacion PeriodoCalificacion { get; set; } = null!;
    public Docente? DocenteQueQualifica { get; set; }
}

namespace ERPEscolar.API.DTOs.ControlEscolar;

/// <summary>
/// DTO para obtener datos de alumno (GET).
/// </summary>
public class AlumnoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CURP { get; set; } = null!;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!;
    public string? Matricula { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public int SchoolId { get; set; }
}

/// <summary>
/// DTO para crear alumno (POST).
/// </summary>
public class CreateAlumnoDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CURP { get; set; } = null!;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!; // M, F
    public string? Direccion { get; set; }
    public string? TelefonoContacto { get; set; }
    public int SchoolId { get; set; }
    public int? TutorId { get; set; } // Tutor opcional en creación
}

/// <summary>
/// DTO para actualizar alumno (PUT).
/// </summary>
public class UpdateAlumnoDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = null!;
}

/// <summary>
/// DTO para respuesta de alumno con datos completos.
/// </summary>
public class AlumnoFullDataDto : AlumnoDto
{
    public List<string> TutoresNombres { get; set; } = [];
    public List<GrupoInscripcionDto> Inscripciones { get; set; } = [];
}

/// <summary>
/// DTO de grupo para inscripción.
/// </summary>
public class GrupoInscripcionDto
{
    public int GrupoId { get; set; }
    public string GrupoNombre { get; set; } = null!;
    public int CicloEscolarId { get; set; }
    public string CicloNombre { get; set; } = null!;
    public bool Activo { get; set; }
}

/// <summary>
/// DTO para paginación de resultados.
/// </summary>
public class PaginatedAlumnosDto
{
    public List<AlumnoDto> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
}

namespace ERPEscolar.API.DTOs.ControlEscolar;

/// <summary>
/// DTO para obtener datos de grupo (GET).
/// </summary>
public class GrupoDto
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int CicloEscolarId { get; set; }
    public string Nombre { get; set; } = null!;
    public int CapacidadMaxima { get; set; }
    public int? DocenteTutorId { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear grupo (POST).
/// </summary>
public class CreateGrupoDto
{
    public int SchoolId { get; set; }
    public int CicloEscolarId { get; set; }
    public string Nombre { get; set; } = null!;
    public int CapacidadMaxima { get; set; }
    public int? DocenteTutorId { get; set; }
}

/// <summary>
/// DTO para actualizar grupo (PUT).
/// </summary>
public class UpdateGrupoDto
{
    public string Nombre { get; set; } = null!;
    public int CapacidadMaxima { get; set; }
    public int? DocenteTutorId { get; set; }
}

/// <summary>
/// DTO para respuesta de grupo con datos completos.
/// </summary>
public class GrupoFullDataDto : GrupoDto
{
    public string? CicloNombre { get; set; }
    public string? DocenteTutorNombre { get; set; }
    public int AlumnosInscritos { get; set; }
}

/// <summary>
/// DTO para paginación de resultados de grupos.
/// </summary>
public class PaginatedGruposDto
{
    public List<GrupoDto> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
}

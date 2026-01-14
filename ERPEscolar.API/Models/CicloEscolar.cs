namespace ERPEscolar.API.Models;

/// <summary>
/// Ciclo escolar (ej: 2024-2025, con periodos y fechas).
/// </summary>
public class CicloEscolar
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public string Nombre { get; set; } = null!; // "2024-2025"
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones
    public School School { get; set; } = null!;
    public ICollection<Grupo> Grupos { get; set; } = [];
    public ICollection<PeriodoCalificacion> Periodos { get; set; } = [];
}

/// <summary>
/// Periodos de calificación dentro de un ciclo escolar.
/// </summary>
public class PeriodoCalificacion
{
    public int Id { get; set; }
    public int CicloEscolarId { get; set; }
    public string Nombre { get; set; } = null!; // "1er Bimestre", "Semestre", etc.
    public int Orden { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaCierre { get; set; }
    public bool Cerrado { get; set; } = false;

    public CicloEscolar CicloEscolar { get; set; } = null!;
    public ICollection<Calificacion> Calificaciones { get; set; } = [];
}

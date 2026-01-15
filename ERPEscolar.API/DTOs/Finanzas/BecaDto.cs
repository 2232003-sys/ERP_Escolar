using System.ComponentModel.DataAnnotations;

namespace ERPEscolar.API.DTOs.Finanzas;

public class BecaDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Justificacion { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation
    public string AlumnoNombre { get; set; } = null!;
}

public class CreateBecaDto
{
    [Required]
    public int AlumnoId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Tipo { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Justificacion { get; set; }
}

public class UpdateBecaDto
{
    [StringLength(100)]
    public string? Nombre { get; set; }

    [StringLength(20)]
    public string? Tipo { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? Valor { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [StringLength(500)]
    public string? Justificacion { get; set; }

    public bool? Activa { get; set; }
}

public class PaginatedBecasDto
{
    public IEnumerable<BecaDto> Becas { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
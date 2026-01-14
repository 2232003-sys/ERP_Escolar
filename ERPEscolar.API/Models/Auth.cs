namespace ERPEscolar.API.Models;

/// <summary>
/// Usuario del sistema (puede ser Docente, Admin, Tutor, etc.).
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool EmailConfirmado { get; set; } = false;
    public bool Activo { get; set; } = true;
    public bool CambioPasswordRequerido { get; set; } = false;
    public DateTime? UltimoLogin { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public Docente? Docente { get; set; }
}

/// <summary>
/// Rol del sistema (Admin, Docente, Tutor, etc.).
/// </summary>
public class Role
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!; // "SuperAdmin", "Docente", etc.
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermiso> RolePermisos { get; set; } = [];
}

/// <summary>
/// Relación entre Usuario y Rol (un usuario puede tener múltiples roles).
/// </summary>
public class UserRole
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

/// <summary>
/// Permiso específico en el sistema.
/// </summary>
public class Permiso
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!; // "Crear.Alumno", "Editar.Calificaciones", etc.
    public string? Descripcion { get; set; }
    public string Recurso { get; set; } = null!; // "Alumnos", "Calificaciones", "Finanzas", etc.
    public string Accion { get; set; } = null!; // "Crear", "Leer", "Editar", "Eliminar"
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<RolePermiso> RolePermisos { get; set; } = [];
}

/// <summary>
/// Relación entre Rol y Permiso (control de acceso basado en roles).
/// </summary>
public class RolePermiso
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermisoId { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public Permiso Permiso { get; set; } = null!;
}

/// <summary>
/// Token de refresco para JWT.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime FechaExpiracion { get; set; }
    public bool Revocado { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

namespace ERPEscolar.API.DTOs.Auth;

public class LoginRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public List<string> Roles { get; set; } = [];
    public List<string> Permisos { get; set; } = [];
}

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = null!;
}

public class CreateUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<int>? RoleIds { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTime FechaCreacion { get; set; }
}

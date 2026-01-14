using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ERPEscolar.API.Models;
using ERPEscolar.API.Data;
using ERPEscolar.API.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<UserDto> CreateUserAsync(CreateUserDto request);
    Task<bool> ValidateTokenAsync(string token);
    Task<int?> GetUserIdFromTokenAsync(string token);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermisos)
            .ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.Activo);

        if (user == null)
        {
            _logger.LogWarning($"Login attempt failed for user: {request.Username}");
            return null;
        }

        // Verificar password (en producción usar BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning($"Invalid password for user: {request.Username}");
            return null;
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Nombre).ToList();
        var permisos = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermisos)
            .Select(rp => rp.Permiso.Nombre)
            .Distinct()
            .ToList();

        var accessToken = GenerateAccessToken(user, roles, permisos);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        user.UltimoLogin = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Roles = roles,
            Permisos = permisos
        };
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermisos)
            .ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.Revocado);

        if (token == null || token.FechaExpiracion < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired refresh token");
            return null;
        }

        var user = token.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Nombre).ToList();
        var permisos = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermisos)
            .Select(rp => rp.Permiso.Nombre)
            .Distinct()
            .ToList();

        var newAccessToken = GenerateAccessToken(user, roles, permisos);
        var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

        // Revocar token antiguo
        token.Revocado = true;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Roles = roles,
            Permisos = permisos
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
            throw new InvalidOperationException("El usuario ya existe");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Asignar roles si se proporcionan
        if (request.RoleIds?.Any() == true)
        {
            foreach (var roleId in request.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId,
                    FechaAsignacion = DateTime.UtcNow
                };
                _context.UserRoles.Add(userRole);
            }
            await _context.SaveChangesAsync();
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Activo = user.Activo,
            FechaCreacion = user.FechaCreacion
        };
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<int?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return Task.FromResult((int?)userId);
            }

            return Task.FromResult((int?)null);
        }
        catch
        {
            return Task.FromResult((int?)null);
        }
    }

    private string GenerateAccessToken(User user, List<string> roles, List<string> permisos)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permiso in permisos)
        {
            claims.Add(new Claim("permiso", permiso));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(int userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString("N"),
            FechaExpiracion = DateTime.UtcNow.AddDays(7),
            Revocado = false,
            FechaCreacion = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }
}

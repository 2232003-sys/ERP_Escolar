using Microsoft.AspNetCore.Mvc;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.DTOs.Auth;

namespace ERPEscolar.API.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión con credenciales
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Error al iniciar sesión" });
        }
    }

    /// <summary>
    /// Renovar token con refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (result == null)
                return Unauthorized(new { message = "Refresh token inválido o expirado" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "Error al renovar token" });
        }
    }

    /// <summary>
    /// Crear nuevo usuario (solo admin)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto request)
    {
        try
        {
            var result = await _authService.CreateUserAsync(request);
            return Created($"api/auth/users/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new { message = "Error al registrar usuario" });
        }
    }

    /// <summary>
    /// Validar token
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }
}

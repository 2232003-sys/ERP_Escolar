namespace ERPEscolar.API.Core.Exceptions;

/// <summary>
/// Excepción cuando un recurso no es encontrado.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string resourceName, int id) 
        : base($"{resourceName} con ID {id} no encontrado.") { }
}

/// <summary>
/// Excepción cuando hay violación de regla de negocio (ej: email duplicado).
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

/// <summary>
/// Excepción para solicitudes incorrectas (ej: operación no permitida).
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

/// <summary>
/// Excepción cuando hay error de validación (ej: campo requerido vacío).
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    
    public ValidationException(Dictionary<string, string[]> errors)
        : base(FormatErrors(errors)) 
    {
        Errors = errors;
    }

    public Dictionary<string, string[]> Errors { get; } = new();

    private static string FormatErrors(Dictionary<string, string[]> errors)
    {
        return "Errores de validación: " + string.Join("; ", 
            errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")));
    }
}

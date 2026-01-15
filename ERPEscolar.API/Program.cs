using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ERPEscolar.API.Data;
using ERPEscolar.API.Infrastructure.Services;
using ERPEscolar.API.Infrastructure.Repositories;
using ERPEscolar.API.DTOs.ControlEscolar;
using ERPEscolar.API.DTOs.Finanzas;
using ERPEscolar.API.DTOs.Fiscal;
using ERPEscolar.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using ERPEscolar.API.Infrastructure.Validators;
using ERPEscolar.API.Infrastructure.Mappings;


var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar JWT con clave por defecto
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "MiClaveSecretaSuperSeguraDeAlMenos32CaracteresParaJWT";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "ERP_Escolar_API",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "ERP_Escolar_Client",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Registrar AutoMapper - incluir todos los profiles
builder.Services.AddAutoMapper(typeof(AlumnoProfile), typeof(GrupoProfile), typeof(InscripcionProfile), typeof(AsistenciaProfile), typeof(CalificacionProfile), typeof(ColegiaturaProfile), typeof(CFDIProfile), typeof(PagoProfile));

// Registrar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAlumnoValidator>();

// Registrar validadores explícitamente para inyección en servicios - Alumno
builder.Services.AddScoped<IValidator<CreateAlumnoDto>, CreateAlumnoValidator>();
builder.Services.AddScoped<IValidator<UpdateAlumnoDto>, UpdateAlumnoValidator>();

// Registrar validadores explícitamente para inyección en servicios - Grupo
builder.Services.AddScoped<IValidator<CreateGrupoDto>, CreateGrupoValidator>();
builder.Services.AddScoped<IValidator<UpdateGrupoDto>, UpdateGrupoValidator>();

// Registrar validadores explícitamente para inyección en servicios - Inscripción
builder.Services.AddScoped<IValidator<CreateInscripcionDto>, CreateInscripcionValidator>();
builder.Services.AddScoped<IValidator<UpdateInscripcionDto>, UpdateInscripcionValidator>();

// Registrar validadores explícitamente para inyección en servicios - Asistencia
builder.Services.AddScoped<IValidator<CreateAsistenciaDto>, CreateAsistenciaValidator>();
builder.Services.AddScoped<IValidator<UpdateAsistenciaDto>, UpdateAsistenciaValidator>();

// Registrar validadores explícitamente para inyección en servicios - Calificación
builder.Services.AddScoped<IValidator<CreateCalificacionDto>, CreateCalificacionValidator>();
builder.Services.AddScoped<IValidator<UpdateCalificacionDto>, UpdateCalificacionValidator>();

// Registrar validadores explícitamente para inyección en servicios - CFDI
builder.Services.AddScoped<IValidator<CreateCFDIDto>, CreateCFDIValidator>();
builder.Services.AddScoped<IValidator<UpdateCFDIDto>, UpdateCFDIValidator>();
builder.Services.AddScoped<IValidator<TimbrarCFDIDto>, TimbrarCFDIValidator>();
builder.Services.AddScoped<IValidator<CancelarCFDIDto>, CancelarCFDIValidator>();

// Registrar validadores explícitamente para inyección en servicios - Pago
builder.Services.AddScoped<IValidator<CreatePagoDto>, CreatePagoValidator>();
builder.Services.AddScoped<IValidator<UpdatePagoDto>, UpdatePagoValidator>();
builder.Services.AddScoped<IValidator<PagoTransferenciaDto>, PagoTransferenciaValidator>();

// Registrar servicios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IGrupoService, GrupoService>();
builder.Services.AddScoped<IInscripcionService, InscripcionService>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
builder.Services.AddScoped<ICalificacionService, CalificacionService>();
builder.Services.AddScoped<IColegiaturaService, ColegiaturaService>();
builder.Services.AddScoped<ICFDIService, CFDIService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IEstadoCuentaService, EstadoCuentaService>();

// API controllers
builder.Services.AddControllers();

// Swagger/OpenAPI con configuración JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// CORS - Política específica para Next.js
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirNextJS", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed data en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await SeedDataService.SeedAsync(context);
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirNextJS");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

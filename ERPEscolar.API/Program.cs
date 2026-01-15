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

// Configurar JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.ASCII.GetBytes(secretKey ?? "");

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Registrar AutoMapper - incluir todos los profiles
builder.Services.AddAutoMapper(typeof(AlumnoProfile), typeof(GrupoProfile), typeof(InscripcionProfile), typeof(AsistenciaProfile), typeof(CargoProfile), typeof(CFDIProfile));

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

// Registrar validadores explícitamente para inyección en servicios - CFDI
builder.Services.AddScoped<IValidator<CreateCFDIDto>, CreateCFDIValidator>();
builder.Services.AddScoped<IValidator<UpdateCFDIDto>, UpdateCFDIValidator>();
builder.Services.AddScoped<IValidator<TimbrarCFDIDto>, TimbrarCFDIValidator>();
builder.Services.AddScoped<IValidator<CancelarCFDIDto>, CancelarCFDIValidator>();

// Registrar servicios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IGrupoService, GrupoService>();
builder.Services.AddScoped<IInscripcionService, InscripcionService>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICFDIService, CFDIService>();

// API controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
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
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

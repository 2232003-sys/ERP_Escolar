using Microsoft.EntityFrameworkCore;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Aquí le decimos que queremos una tabla de "Alumnos"
    public DbSet<Alumno> Alumnos { get; set; }
}
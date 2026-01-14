using Microsoft.EntityFrameworkCore;
using ERPEscolar.API.Models;

namespace ERPEscolar.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Control Escolar
    public DbSet<School> Schools { get; set; }
    public DbSet<CicloEscolar> CiclosEscolares { get; set; }
    public DbSet<PeriodoCalificacion> PeriodosCalificacion { get; set; }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Docente> Docentes { get; set; }
    public DbSet<Materia> Materias { get; set; }
    public DbSet<Grupo> Grupos { get; set; }
    public DbSet<GrupoMateria> GrupoMaterias { get; set; }
    public DbSet<Inscripcion> Inscripciones { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }

    // Autenticación y Autorización
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<RolePermiso> RolePermisos { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Finanzas
    public DbSet<ConceptoCobro> ConceptosCobro { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Beca> Becas { get; set; }
    public DbSet<ConfiguracionFiscal> ConfiguracionesFiscales { get; set; }

    // Fiscal (CFDI + IEDU)
    public DbSet<CFDI> CFDIs { get; set; }
    public DbSet<ComplementoEducativo> ComplementosEducativos { get; set; }
    public DbSet<BitacoraFiscal> BitacorasFiscales { get; set; }
    public DbSet<ConfiguracionCFDI> ConfiguracionesCFDI { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar índices y relaciones
        
        // School
        modelBuilder.Entity<School>()
            .HasIndex(s => s.RFC)
            .IsUnique();

        // Alumno
        modelBuilder.Entity<Alumno>()
            .HasIndex(a => a.CURP)
            .IsUnique();
        modelBuilder.Entity<Alumno>()
            .HasIndex(a => a.Matricula)
            .IsUnique();
        modelBuilder.Entity<Alumno>()
            .HasOne(a => a.School)
            .WithMany(s => s.Alumnos)
            .HasForeignKey(a => a.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tutor - relación muchos a muchos con Alumno
        modelBuilder.Entity<Alumno>()
            .HasMany(a => a.Tutores)
            .WithMany(t => t.Alumnos);

        // Docente
        modelBuilder.Entity<Docente>()
            .HasOne(d => d.School)
            .WithMany(s => s.Docentes)
            .HasForeignKey(d => d.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Docente>()
            .HasOne(d => d.User)
            .WithOne(u => u.Docente)
            .HasForeignKey<Docente>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Materia
        modelBuilder.Entity<Materia>()
            .HasOne(m => m.School)
            .WithMany()
            .HasForeignKey(m => m.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Materia>()
            .HasIndex(m => new { m.SchoolId, m.Clave })
            .IsUnique();

        // Grupo
        modelBuilder.Entity<Grupo>()
            .HasOne(g => g.School)
            .WithMany(s => s.Grupos)
            .HasForeignKey(g => g.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Grupo>()
            .HasOne(g => g.CicloEscolar)
            .WithMany(c => c.Grupos)
            .HasForeignKey(g => g.CicloEscolarId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Grupo>()
            .HasIndex(g => new { g.SchoolId, g.CicloEscolarId, g.Nombre })
            .IsUnique();

        // GrupoMateria
        modelBuilder.Entity<GrupoMateria>()
            .HasIndex(gm => new { gm.GrupoId, gm.MateriaId, gm.DocenteId })
            .IsUnique();

        // Inscripción
        modelBuilder.Entity<Inscripcion>()
            .HasIndex(i => new { i.AlumnoId, i.GrupoId, i.CicloEscolarId })
            .IsUnique();

        // Auth
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Finanzas
        modelBuilder.Entity<ConceptoCobro>()
            .HasOne(cc => cc.School)
            .WithMany(s => s.ConceptosCobro)
            .HasForeignKey(cc => cc.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasIndex(c => c.Folio)
            .IsUnique();

        modelBuilder.Entity<Pago>()
            .HasIndex(p => p.Folio)
            .IsUnique();

        // Fiscal
        modelBuilder.Entity<CFDI>()
            .HasIndex(c => c.UUID)
            .IsUnique();
        modelBuilder.Entity<CFDI>()
            .HasIndex(c => new { c.Serie, c.Folio })
            .IsUnique();

        modelBuilder.Entity<ConfiguracionFiscal>()
            .HasOne(cf => cf.School)
            .WithMany()
            .HasForeignKey(cf => cf.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ConfiguracionCFDI>()
            .HasOne(cc => cc.School)
            .WithMany()
            .HasForeignKey(cc => cc.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComplementoEducativo>()
            .HasOne(ce => ce.CFDI)
            .WithMany()
            .HasForeignKey(ce => ce.CFDIId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BitacoraFiscal>()
            .HasIndex(bf => bf.Timestamp);
    }
}
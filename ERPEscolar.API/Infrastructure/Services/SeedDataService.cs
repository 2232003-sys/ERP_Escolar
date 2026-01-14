using ERPEscolar.API.Data;
using ERPEscolar.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPEscolar.API.Infrastructure.Services;

public static class SeedDataService
{
    public static async Task SeedAsync(AppDbContext context)
    {
        try
        {
            // Aplicar migraciones pendientes
            await context.Database.MigrateAsync();

            // Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Nombre = "SuperAdmin", Descripcion = "Administrador supremo", Activo = true },
                    new Role { Nombre = "Admin TI", Descripcion = "Administrador de TI", Activo = true },
                    new Role { Nombre = "Dirección", Descripcion = "Dirección escolar", Activo = true },
                    new Role { Nombre = "Control Escolar", Descripcion = "Control escolar", Activo = true },
                    new Role { Nombre = "Docente", Descripcion = "Docente", Activo = true },
                    new Role { Nombre = "Finanzas", Descripcion = "Finanzas/Contabilidad", Activo = true },
                    new Role { Nombre = "Tutor", Descripcion = "Padre/Tutor", Activo = true },
                    new Role { Nombre = "Alumno", Descripcion = "Alumno (lectura)", Activo = true }
                };
                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Roles creados");
            }

            // Seed Permisos
            if (!await context.Permisos.AnyAsync())
            {
                var permisos = new List<Permiso>
                {
                    // Control Escolar
                    new Permiso { Nombre = "Crear.Alumno", Recurso = "Alumnos", Accion = "Crear", Activo = true },
                    new Permiso { Nombre = "Editar.Alumno", Recurso = "Alumnos", Accion = "Editar", Activo = true },
                    new Permiso { Nombre = "Leer.Alumno", Recurso = "Alumnos", Accion = "Leer", Activo = true },
                    new Permiso { Nombre = "Eliminar.Alumno", Recurso = "Alumnos", Accion = "Eliminar", Activo = true },
                    
                    // Calificaciones
                    new Permiso { Nombre = "Crear.Calificacion", Recurso = "Calificaciones", Accion = "Crear", Activo = true },
                    new Permiso { Nombre = "Editar.Calificacion", Recurso = "Calificaciones", Accion = "Editar", Activo = true },
                    new Permiso { Nombre = "Leer.Calificacion", Recurso = "Calificaciones", Accion = "Leer", Activo = true },
                    
                    // Finanzas
                    new Permiso { Nombre = "Crear.Cargo", Recurso = "Finanzas", Accion = "Crear", Activo = true },
                    new Permiso { Nombre = "Registrar.Pago", Recurso = "Finanzas", Accion = "Crear", Activo = true },
                    new Permiso { Nombre = "Leer.EstadoCuenta", Recurso = "Finanzas", Accion = "Leer", Activo = true },
                    
                    // Fiscal
                    new Permiso { Nombre = "Generar.CFDI", Recurso = "Fiscal", Accion = "Crear", Activo = true },
                    new Permiso { Nombre = "Timbrar.CFDI", Recurso = "Fiscal", Accion = "Crear", Activo = true }
                };
                await context.Permisos.AddRangeAsync(permisos);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Permisos creados");
            }

            // Asignar permisos a roles
            if (!await context.RolePermisos.AnyAsync())
            {
                var superAdminRole = await context.Roles.FirstAsync(r => r.Nombre == "SuperAdmin");
                var allPermisos = await context.Permisos.ToListAsync();

                foreach (var permiso in allPermisos)
                {
                    context.RolePermisos.Add(new RolePermiso
                    {
                        RoleId = superAdminRole.Id,
                        PermisoId = permiso.Id
                    });
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Permisos asignados a SuperAdmin");
            }

            // Seed Admin User
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@erp.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    EmailConfirmado = true,
                    Activo = true,
                    CambioPasswordRequerido = false
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                // Asignar rol SuperAdmin
                var superAdminRole = await context.Roles.FirstAsync(r => r.Nombre == "SuperAdmin");
                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = superAdminRole.Id
                });
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Usuario admin creado (Password: Admin123!)");
            }

            // Seed School
            if (!await context.Schools.AnyAsync())
            {
                var school = new School
                {
                    Nombre = "Instituto Educativo Demo",
                    RFC = "IED000000A10",
                    ClaveCT = "25PPR0001A",
                    Direccion = "Calle Principal 123, Guadalajara, Jalisco",
                    Telefono = "33-1234-5678",
                    Email = "contacto@iedr.edu.mx",
                    Activo = true
                };
                context.Schools.Add(school);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Escuela creada");

                // Seed Ciclo Escolar
                var ciclo = new CicloEscolar
                {
                    SchoolId = school.Id,
                    Nombre = "2024-2025",
                    FechaInicio = new DateTime(2024, 8, 15),
                    FechaFin = new DateTime(2025, 6, 30),
                    Activo = true
                };
                context.CiclosEscolares.Add(ciclo);
                await context.SaveChangesAsync();

                // Seed Periodos
                var periodos = new List<PeriodoCalificacion>
                {
                    new PeriodoCalificacion
                    {
                        CicloEscolarId = ciclo.Id,
                        Nombre = "1er Bimestre",
                        Orden = 1,
                        FechaInicio = new DateTime(2024, 8, 15),
                        FechaFin = new DateTime(2024, 10, 15),
                        FechaCierre = new DateTime(2024, 10, 20),
                        Cerrado = false
                    },
                    new PeriodoCalificacion
                    {
                        CicloEscolarId = ciclo.Id,
                        Nombre = "2do Bimestre",
                        Orden = 2,
                        FechaInicio = new DateTime(2024, 10, 16),
                        FechaFin = new DateTime(2024, 12, 15),
                        FechaCierre = new DateTime(2024, 12, 20),
                        Cerrado = false
                    }
                };
                context.PeriodosCalificacion.AddRange(periodos);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Ciclo escolar y períodos creados");

                // Seed Materias
                var materias = new List<Materia>
                {
                    new Materia { SchoolId = school.Id, Nombre = "Matemáticas", Clave = "MAT-01", Creditos = 4 },
                    new Materia { SchoolId = school.Id, Nombre = "Español", Clave = "ESP-01", Creditos = 3 },
                    new Materia { SchoolId = school.Id, Nombre = "Ciencias", Clave = "CIE-01", Creditos = 4 },
                    new Materia { SchoolId = school.Id, Nombre = "Historia", Clave = "HIS-01", Creditos = 2 },
                    new Materia { SchoolId = school.Id, Nombre = "Educación Física", Clave = "EDF-01", Creditos = 2 }
                };
                context.Materias.AddRange(materias);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Materias creadas");

                // Seed Docentes (User + Docente)
                var docenteRole = await context.Roles.FirstAsync(r => r.Nombre == "Docente");
                var docentes = new List<User>();

                for (int i = 1; i <= 3; i++)
                {
                    var user = new User
                    {
                        Username = $"docente{i}",
                        Email = $"docente{i}@erp.local",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Docente123!"),
                        EmailConfirmado = true,
                        Activo = true
                    };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();

                    context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = docenteRole.Id });

                    var docente = new Docente
                    {
                        SchoolId = school.Id,
                        UserId = user.Id,
                        Nombre = $"Docente{i}",
                        Apellido = "Demo",
                        Email = $"docente{i}@erp.local",
                        RFC = $"DOC000000{i}00",
                        FechaContratacion = DateTime.UtcNow.AddYears(-2),
                        Activo = true
                    };
                    context.Docentes.Add(docente);
                    docentes.Add(user);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Docentes creados");

                // Seed Tutores
                var tutores = new List<Tutor>();
                for (int i = 1; i <= 5; i++)
                {
                    var tutor = new Tutor
                    {
                        Nombre = $"Tutor{i}",
                        Apellido = "Demo",
                        Email = $"tutor{i}@erp.local",
                        Telefono = $"33-1234-567{i}",
                        RFC = $"TUT000000{i}00",
                        Parentesco = "Padre",
                        PrincipalResponsable = true,
                        Activo = true
                    };
                    context.Tutores.Add(tutor);
                    tutores.Add(tutor);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Tutores creados");

                // Seed Alumnos
                var alumnoRole = await context.Roles.FirstAsync(r => r.Nombre == "Alumno");
                for (int i = 1; i <= 10; i++)
                {
                    var alumnoUser = new User
                    {
                        Username = $"alumno{i}",
                        Email = $"alumno{i}@erp.local",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Alumno123!"),
                        EmailConfirmado = true,
                        Activo = true
                    };
                    context.Users.Add(alumnoUser);
                    await context.SaveChangesAsync();
                    context.UserRoles.Add(new UserRole { UserId = alumnoUser.Id, RoleId = alumnoRole.Id });

                    var alumno = new Alumno
                    {
                        SchoolId = school.Id,
                        Nombre = $"Alumno{i}",
                        Apellido = "Demo",
                        Email = $"alumno{i}@erp.local",
                        CURP = $"ALUD000000{i:D2}000",
                        FechaNacimiento = new DateTime(2010, 1, 1).AddDays(i * 10),
                        Sexo = i % 2 == 0 ? "M" : "F",
                        Matricula = $"ALU-2024-{i:D5}",
                        Activo = true
                    };
                    context.Alumnos.Add(alumno);
                    
                    // Asignar tutor aleatorio
                    alumno.Tutores.Add(tutores[i % tutores.Count]);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Alumnos creados");

                // Seed Grupos
                var docente1 = await context.Docentes.FirstAsync();
                var grupo = new Grupo
                {
                    SchoolId = school.Id,
                    CicloEscolarId = ciclo.Id,
                    Nombre = "1ro A",
                    Grado = "1ro",
                    Seccion = "A",
                    DocenteTutorId = docente1.Id,
                    CapacidadMaxima = 35,
                    Activo = true
                };
                context.Grupos.Add(grupo);
                await context.SaveChangesAsync();

                // Seed GrupoMaterias
                var materiasList = await context.Materias.ToListAsync();
                var docentesList = await context.Docentes.ToListAsync();

                foreach (var materia in materiasList.Take(3))
                {
                    var gm = new GrupoMateria
                    {
                        GrupoId = grupo.Id,
                        MateriaId = materia.Id,
                        DocenteId = docentesList[Random.Shared.Next(docentesList.Count)].Id,
                        Peso = 1,
                        Activo = true
                    };
                    context.GrupoMaterias.Add(gm);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Grupos y materias asignadas");

                // Seed Inscripciones
                var alumnos = await context.Alumnos.ToListAsync();
                foreach (var alumno in alumnos.Take(5))
                {
                    var inscripcion = new Inscripcion
                    {
                        AlumnoId = alumno.Id,
                        GrupoId = grupo.Id,
                        CicloEscolarId = ciclo.Id,
                        FechaInscripcion = DateTime.UtcNow,
                        Activo = true
                    };
                    context.Inscripciones.Add(inscripcion);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Inscripciones creadas");

                // Seed Conceptos de Cobro
                var conceptos = new List<ConceptoCobro>
                {
                    new ConceptoCobro { SchoolId = school.Id, Nombre = "Cuota Mensual", Clave = "CUO-01", MontoBase = 2500, TipoConcepto = "Fijo" },
                    new ConceptoCobro { SchoolId = school.Id, Nombre = "Inscripción", Clave = "INS-01", MontoBase = 3000, TipoConcepto = "Variable" },
                    new ConceptoCobro { SchoolId = school.Id, Nombre = "Material Didáctico", Clave = "MAT-02", MontoBase = 500, TipoConcepto = "Fijo" }
                };
                context.ConceptosCobro.AddRange(conceptos);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Conceptos de cobro creados");

                // Seed Cargos
                foreach (var alumno in alumnos.Take(5))
                {
                    var cargo = new Cargo
                    {
                        AlumnoId = alumno.Id,
                        ConceptoCobroId = conceptos[0].Id,
                        CicloEscolarId = ciclo.Id,
                        Folio = $"CAR-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}",
                        Mes = "2024-10",
                        Monto = 2500,
                        Descuento = 0,
                        Recargo = 0,
                        IVA = 400,
                        Estado = "Pendiente",
                        FechaEmision = DateTime.UtcNow,
                        FechaVencimiento = DateTime.UtcNow.AddDays(30)
                    };
                    context.Cargos.Add(cargo);
                }
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Cargos creados");

                Console.WriteLine("\n✅ SEED DATA COMPLETADO");
                Console.WriteLine("\nCredenciales de prueba:");
                Console.WriteLine("  Admin: admin / Admin123!");
                Console.WriteLine("  Docente: docente1 / Docente123!");
                Console.WriteLine("  Alumno: alumno1 / Alumno123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en seed data: {ex.Message}");
            throw;
        }
    }
}

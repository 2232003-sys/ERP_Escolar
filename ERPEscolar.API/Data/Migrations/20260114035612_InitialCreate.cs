using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERPEscolar.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Recurso = table.Column<string>(type: "text", nullable: false),
                    Accion = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    RFC = table.Column<string>(type: "text", nullable: true),
                    ClaveCT = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    RFC = table.Column<string>(type: "text", nullable: true),
                    CURP = table.Column<string>(type: "text", nullable: true),
                    Parentesco = table.Column<string>(type: "text", nullable: false),
                    PrincipalResponsable = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    EmailConfirmado = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CambioPasswordRequerido = table.Column<bool>(type: "boolean", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermisoId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermisos_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermisos_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CURP = table.Column<string>(type: "text", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: false),
                    Matricula = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alumnos_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CiclosEscolares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiclosEscolares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CiclosEscolares_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConceptosCobro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Clave = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    MontoBase = table.Column<decimal>(type: "numeric", nullable: false),
                    TipoConcepto = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptosCobro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptosCobro_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesCFDI",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    ProvedorTimbrado = table.Column<string>(type: "text", nullable: false),
                    UrlWSTimbrado = table.Column<string>(type: "text", nullable: false),
                    UrlWSCancelacion = table.Column<string>(type: "text", nullable: false),
                    Usuario_PAC = table.Column<string>(type: "text", nullable: false),
                    Contrasena_PAC = table.Column<string>(type: "text", nullable: false),
                    NombreComercial = table.Column<string>(type: "text", nullable: false),
                    LogoUrlRuta = table.Column<string>(type: "text", nullable: false),
                    DiasCadenaOriginal = table.Column<int>(type: "integer", nullable: false),
                    ReintentosMaximos = table.Column<int>(type: "integer", nullable: false),
                    TiempoEsperaReintentos = table.Column<int>(type: "integer", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesCFDI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesCFDI_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    RFC = table.Column<string>(type: "text", nullable: false),
                    RazonSocial = table.Column<string>(type: "text", nullable: false),
                    RegimenFiscal = table.Column<string>(type: "text", nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeISR = table.Column<decimal>(type: "numeric", nullable: false),
                    CertificadoRutaArchivo = table.Column<string>(type: "text", nullable: false),
                    ClavePrivadaRutaArchivo = table.Column<string>(type: "text", nullable: false),
                    ClavePrivada = table.Column<string>(type: "text", nullable: false),
                    ProveedorTimbrado = table.Column<string>(type: "text", nullable: false),
                    TokenTimbrado = table.Column<string>(type: "text", nullable: true),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesFiscales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesFiscales_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    RFC = table.Column<string>(type: "text", nullable: true),
                    CURP = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaContratacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Docentes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Docentes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revocado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlumnoTutor",
                columns: table => new
                {
                    AlumnosId = table.Column<int>(type: "integer", nullable: false),
                    TutoresId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoTutor", x => new { x.AlumnosId, x.TutoresId });
                    table.ForeignKey(
                        name: "FK_AlumnoTutor_Alumnos_AlumnosId",
                        column: x => x.AlumnosId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnoTutor_Tutores_TutoresId",
                        column: x => x.TutoresId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Becas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Justificacion = table.Column<string>(type: "text", nullable: true),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Becas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Becas_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosCalificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CicloEscolarId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cerrado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosCalificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodosCalificacion_CiclosEscolares_CicloEscolarId",
                        column: x => x.CicloEscolarId,
                        principalTable: "CiclosEscolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    ConceptoCobroId = table.Column<int>(type: "integer", nullable: false),
                    CicloEscolarId = table.Column<int>(type: "integer", nullable: false),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    Mes = table.Column<string>(type: "text", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Recargo = table.Column<decimal>(type: "numeric", nullable: false),
                    IVA = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    MontoRecibido = table.Column<decimal>(type: "numeric", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cargos_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cargos_CiclosEscolares_CicloEscolarId",
                        column: x => x.CicloEscolarId,
                        principalTable: "CiclosEscolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cargos_ConceptosCobro_ConceptoCobroId",
                        column: x => x.ConceptoCobroId,
                        principalTable: "ConceptosCobro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    CicloEscolarId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Grado = table.Column<string>(type: "text", nullable: false),
                    Seccion = table.Column<string>(type: "text", nullable: false),
                    DocenteTutorId = table.Column<int>(type: "integer", nullable: true),
                    CapacidadMaxima = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grupos_CiclosEscolares_CicloEscolarId",
                        column: x => x.CicloEscolarId,
                        principalTable: "CiclosEscolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grupos_Docentes_DocenteTutorId",
                        column: x => x.DocenteTutorId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grupos_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Clave = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Creditos = table.Column<decimal>(type: "numeric", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocenteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materias_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Materias_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CFDIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CargoId = table.Column<int>(type: "integer", nullable: false),
                    UUID = table.Column<string>(type: "text", nullable: false),
                    Serie = table.Column<string>(type: "text", nullable: false),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    RFC_Emisor = table.Column<string>(type: "text", nullable: false),
                    RFC_Receptor = table.Column<string>(type: "text", nullable: false),
                    NombreReceptor = table.Column<string>(type: "text", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    IVA = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    RazonCancelacion = table.Column<string>(type: "text", nullable: true),
                    RutaXML = table.Column<string>(type: "text", nullable: true),
                    RutaPDF = table.Column<string>(type: "text", nullable: true),
                    XMLConTimbrado = table.Column<string>(type: "text", nullable: true),
                    NivelEducativo = table.Column<string>(type: "text", nullable: true),
                    CURP_Alumno = table.Column<string>(type: "text", nullable: true),
                    ClaveCT = table.Column<string>(type: "text", nullable: true),
                    CadenaOriginal = table.Column<string>(type: "text", nullable: true),
                    FechaTimbrado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCancelacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorTimbrado = table.Column<string>(type: "text", nullable: true),
                    ReintentosTimbrado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CFDIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CFDIs_Cargos_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    CargoId = table.Column<int>(type: "integer", nullable: true),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Metodo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    ReferenciaExterna = table.Column<string>(type: "text", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaDepósito = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BancoOrigen = table.Column<string>(type: "text", nullable: true),
                    Comprobante = table.Column<string>(type: "text", nullable: true),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagos_Cargos_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    GrupoId = table.Column<int>(type: "integer", nullable: false),
                    CicloEscolarId = table.Column<int>(type: "integer", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripciones_CiclosEscolares_CicloEscolarId",
                        column: x => x.CicloEscolarId,
                        principalTable: "CiclosEscolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoMaterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrupoId = table.Column<int>(type: "integer", nullable: false),
                    MateriaId = table.Column<int>(type: "integer", nullable: false),
                    DocenteId = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoMaterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoMaterias_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoMaterias_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoMaterias_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BitacorasFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CFDIId = table.Column<int>(type: "integer", nullable: true),
                    CargoId = table.Column<int>(type: "integer", nullable: true),
                    Evento = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    DetalleError = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    IPOrigen = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacorasFiscales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacorasFiscales_CFDIs_CFDIId",
                        column: x => x.CFDIId,
                        principalTable: "CFDIs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BitacorasFiscales_Cargos_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComplementosEducativos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CFDIId = table.Column<int>(type: "integer", nullable: false),
                    NivelEducativo = table.Column<string>(type: "text", nullable: false),
                    CURP = table.Column<string>(type: "text", nullable: false),
                    ClaveCT = table.Column<string>(type: "text", nullable: false),
                    NoExamenGeneral = table.Column<string>(type: "text", nullable: true),
                    FechaExpedicion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidadoSAT = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplementosEducativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplementosEducativos_CFDIs_CFDIId",
                        column: x => x.CFDIId,
                        principalTable: "CFDIs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    GrupoMateriaId = table.Column<int>(type: "integer", nullable: false),
                    DocenteQueRegistroId = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asistencias_Docentes_DocenteQueRegistroId",
                        column: x => x.DocenteQueRegistroId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistencias_GrupoMaterias_GrupoMateriaId",
                        column: x => x.GrupoMateriaId,
                        principalTable: "GrupoMaterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlumnoId = table.Column<int>(type: "integer", nullable: false),
                    GrupoMateriaId = table.Column<int>(type: "integer", nullable: false),
                    PeriodoCalificacionId = table.Column<int>(type: "integer", nullable: false),
                    Calificacion1 = table.Column<decimal>(type: "numeric", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    Aprobado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCalificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocenteQueQualificaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Docentes_DocenteQueQualificaId",
                        column: x => x.DocenteQueQualificaId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Calificaciones_GrupoMaterias_GrupoMateriaId",
                        column: x => x.GrupoMateriaId,
                        principalTable: "GrupoMaterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calificaciones_PeriodosCalificacion_PeriodoCalificacionId",
                        column: x => x.PeriodoCalificacionId,
                        principalTable: "PeriodosCalificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_CURP",
                table: "Alumnos",
                column: "CURP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_Matricula",
                table: "Alumnos",
                column: "Matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_SchoolId",
                table: "Alumnos",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoTutor_TutoresId",
                table: "AlumnoTutor",
                column: "TutoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_AlumnoId",
                table: "Asistencias",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_DocenteQueRegistroId",
                table: "Asistencias",
                column: "DocenteQueRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_GrupoMateriaId",
                table: "Asistencias",
                column: "GrupoMateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Becas_AlumnoId",
                table: "Becas",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacorasFiscales_CargoId",
                table: "BitacorasFiscales",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacorasFiscales_CFDIId",
                table: "BitacorasFiscales",
                column: "CFDIId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacorasFiscales_Timestamp",
                table: "BitacorasFiscales",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_AlumnoId",
                table: "Calificaciones",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_DocenteQueQualificaId",
                table: "Calificaciones",
                column: "DocenteQueQualificaId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_GrupoMateriaId",
                table: "Calificaciones",
                column: "GrupoMateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_PeriodoCalificacionId",
                table: "Calificaciones",
                column: "PeriodoCalificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_AlumnoId",
                table: "Cargos",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_CicloEscolarId",
                table: "Cargos",
                column: "CicloEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_ConceptoCobroId",
                table: "Cargos",
                column: "ConceptoCobroId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_Folio",
                table: "Cargos",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CFDIs_CargoId",
                table: "CFDIs",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_CFDIs_Serie_Folio",
                table: "CFDIs",
                columns: new[] { "Serie", "Folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CFDIs_UUID",
                table: "CFDIs",
                column: "UUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CiclosEscolares_SchoolId",
                table: "CiclosEscolares",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplementosEducativos_CFDIId",
                table: "ComplementosEducativos",
                column: "CFDIId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptosCobro_SchoolId",
                table: "ConceptosCobro",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesCFDI_SchoolId",
                table: "ConfiguracionesCFDI",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesFiscales_SchoolId",
                table: "ConfiguracionesFiscales",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_SchoolId",
                table: "Docentes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_UserId",
                table: "Docentes",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrupoMaterias_DocenteId",
                table: "GrupoMaterias",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoMaterias_GrupoId_MateriaId_DocenteId",
                table: "GrupoMaterias",
                columns: new[] { "GrupoId", "MateriaId", "DocenteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrupoMaterias_MateriaId",
                table: "GrupoMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_CicloEscolarId",
                table: "Grupos",
                column: "CicloEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_DocenteTutorId",
                table: "Grupos",
                column: "DocenteTutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_SchoolId_CicloEscolarId_Nombre",
                table: "Grupos",
                columns: new[] { "SchoolId", "CicloEscolarId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_AlumnoId_GrupoId_CicloEscolarId",
                table: "Inscripciones",
                columns: new[] { "AlumnoId", "GrupoId", "CicloEscolarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_CicloEscolarId",
                table: "Inscripciones",
                column: "CicloEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_GrupoId",
                table: "Inscripciones",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_DocenteId",
                table: "Materias",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_SchoolId_Clave",
                table: "Materias",
                columns: new[] { "SchoolId", "Clave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_AlumnoId",
                table: "Pagos",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_CargoId",
                table: "Pagos",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_Folio",
                table: "Pagos",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosCalificacion_CicloEscolarId",
                table: "PeriodosCalificacion",
                column: "CicloEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermisos_PermisoId",
                table: "RolePermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermisos_RoleId",
                table: "RolePermisos",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_RFC",
                table: "Schools",
                column: "RFC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoTutor");

            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Becas");

            migrationBuilder.DropTable(
                name: "BitacorasFiscales");

            migrationBuilder.DropTable(
                name: "Calificaciones");

            migrationBuilder.DropTable(
                name: "ComplementosEducativos");

            migrationBuilder.DropTable(
                name: "ConfiguracionesCFDI");

            migrationBuilder.DropTable(
                name: "ConfiguracionesFiscales");

            migrationBuilder.DropTable(
                name: "Inscripciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermisos");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Tutores");

            migrationBuilder.DropTable(
                name: "GrupoMaterias");

            migrationBuilder.DropTable(
                name: "PeriodosCalificacion");

            migrationBuilder.DropTable(
                name: "CFDIs");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "CiclosEscolares");

            migrationBuilder.DropTable(
                name: "ConceptosCobro");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Schools");
        }
    }
}

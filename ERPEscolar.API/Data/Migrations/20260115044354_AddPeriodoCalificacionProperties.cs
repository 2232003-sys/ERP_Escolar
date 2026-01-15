using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPEscolar.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodoCalificacionProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Alumnos_AlumnoId",
                table: "Asistencias");

            migrationBuilder.RenameColumn(
                name: "Observacion",
                table: "Asistencias",
                newName: "RegistradoPor");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "PeriodosCalificacion",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "PeriodosCalificacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Clave",
                table: "PeriodosCalificacion",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "PeriodosCalificacion",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "PeriodosCalificacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Pagos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Pagos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Inscripciones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Cargos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "InscripcionId",
                table: "Calificaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AlumnoId",
                table: "Asistencias",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Asistencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InscripcionId",
                table: "Asistencias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Asistencias",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosCalificacion_SchoolId",
                table: "PeriodosCalificacion",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_InscripcionId",
                table: "Calificaciones",
                column: "InscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_InscripcionId",
                table: "Asistencias",
                column: "InscripcionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Alumnos_AlumnoId",
                table: "Asistencias",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Inscripciones_InscripcionId",
                table: "Asistencias",
                column: "InscripcionId",
                principalTable: "Inscripciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Inscripciones_InscripcionId",
                table: "Calificaciones",
                column: "InscripcionId",
                principalTable: "Inscripciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosCalificacion_Schools_SchoolId",
                table: "PeriodosCalificacion",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Alumnos_AlumnoId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Inscripciones_InscripcionId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Inscripciones_InscripcionId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodosCalificacion_Schools_SchoolId",
                table: "PeriodosCalificacion");

            migrationBuilder.DropIndex(
                name: "IX_PeriodosCalificacion_SchoolId",
                table: "PeriodosCalificacion");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_InscripcionId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_InscripcionId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "PeriodosCalificacion");

            migrationBuilder.DropColumn(
                name: "Clave",
                table: "PeriodosCalificacion");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "PeriodosCalificacion");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "PeriodosCalificacion");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "InscripcionId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "InscripcionId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Asistencias");

            migrationBuilder.RenameColumn(
                name: "RegistradoPor",
                table: "Asistencias",
                newName: "Observacion");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "PeriodosCalificacion",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AlumnoId",
                table: "Asistencias",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Alumnos_AlumnoId",
                table: "Asistencias",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppVidaSana.Migrations
{
    /// <inheritdoc />
    public partial class Azurebd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Ejercicios",
                columns: table => new
                {
                    idejercicio = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    intensidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tiempo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejercicios", x => x.idejercicio);
                    table.ForeignKey(
                        name: "FK_Ejercicios_Cuentas_id",
                        column: x => x.id,
                        principalTable: "Cuentas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fechanacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estatura = table.Column<int>(type: "int", nullable: false),
                    peso = table.Column<int>(type: "int", nullable: false),
                    protocolo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_Perfiles_Cuentas_id",
                        column: x => x.id,
                        principalTable: "Cuentas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SegMenEjercicios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    pregunta1 = table.Column<int>(type: "int", nullable: false),
                    pregunta2 = table.Column<int>(type: "int", nullable: false),
                    pregunta3 = table.Column<int>(type: "int", nullable: false),
                    pregunta4 = table.Column<int>(type: "int", nullable: false),
                    pregunta5 = table.Column<int>(type: "int", nullable: false),
                    pregunta6 = table.Column<int>(type: "int", nullable: false),
                    pregunta7 = table.Column<int>(type: "int", nullable: false),
                    actcaminata = table.Column<float>(type: "real", nullable: false),
                    afmoderada = table.Column<float>(type: "real", nullable: false),
                    afvigorosa = table.Column<float>(type: "real", nullable: false),
                    totalMET = table.Column<float>(type: "real", nullable: false),
                    conductasend = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nivelAF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegMenEjercicios", x => x.id);
                    table.ForeignKey(
                        name: "FK_SegMenEjercicios_Cuentas_id",
                        column: x => x.id,
                        principalTable: "Cuentas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_id",
                table: "Ejercicios",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ejercicios");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "SegMenEjercicios");

            migrationBuilder.DropTable(
                name: "Cuentas");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppVidaSana.Migrations
{
    /// <inheritdoc />
    public partial class AzureBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.cuentaID);
                });

            migrationBuilder.CreateTable(
                name: "Almuerzos",
                columns: table => new
                {
                    almuerzoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    nivelSaciedad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emocionesLigadas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almuerzos", x => x.almuerzoID);
                    table.ForeignKey(
                        name: "FK_Almuerzos_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cenas",
                columns: table => new
                {
                    cenaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    nivelSaciedad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emocionesLigadas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cenas", x => x.cenaID);
                    table.ForeignKey(
                        name: "FK_Cenas_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Colaciones",
                columns: table => new
                {
                    colacionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    nivelSaciedad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emocionesLigadas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaciones", x => x.colacionID);
                    table.ForeignKey(
                        name: "FK_Colaciones_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comidas",
                columns: table => new
                {
                    comidaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    nivelSaciedad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emocionesLigadas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comidas", x => x.comidaID);
                    table.ForeignKey(
                        name: "FK_Comidas_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Desayunos",
                columns: table => new
                {
                    desayunoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    nivelSaciedad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emocionesLigadas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desayunos", x => x.desayunoID);
                    table.ForeignKey(
                        name: "FK_Desayunos_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "efectoSecundarios",
                columns: table => new
                {
                    registroID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    horarioInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    horarioFinal = table.Column<TimeOnly>(type: "time", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_efectoSecundarios", x => x.registroID);
                    table.ForeignKey(
                        name: "FK_efectoSecundarios_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ejercicios",
                columns: table => new
                {
                    ejercicioID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    intensidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tiempo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejercicios", x => x.ejercicioID);
                    table.ForeignKey(
                        name: "FK_Ejercicios_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitosBebida",
                columns: table => new
                {
                    habitoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    tipoBebida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitosBebida", x => x.habitoID);
                    table.ForeignKey(
                        name: "FK_habitosBebida_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitosDroga",
                columns: table => new
                {
                    habitoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    cigarrosConsumidos = table.Column<int>(type: "int", nullable: false),
                    estadoEmocionalPredominante = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitosDroga", x => x.habitoID);
                    table.ForeignKey(
                        name: "FK_habitosDroga_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "habitosSueño",
                columns: table => new
                {
                    habitoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    horasSueño = table.Column<int>(type: "int", nullable: false),
                    percepcionDescanso = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habitosSueño", x => x.habitoID);
                    table.ForeignKey(
                        name: "FK_habitosSueño_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicamentos",
                columns: table => new
                {
                    medicamentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    nombreMedicamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dosis = table.Column<int>(type: "int", nullable: false),
                    frecuenciaSem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    frecuenciaDiaria = table.Column<int>(type: "int", nullable: false),
                    horario1 = table.Column<TimeOnly>(type: "time", nullable: false),
                    horario2 = table.Column<TimeOnly>(type: "time", nullable: false),
                    horario3 = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamentos", x => x.medicamentoID);
                    table.ForeignKey(
                        name: "FK_Medicamentos_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estatura = table.Column<int>(type: "int", nullable: false),
                    peso = table.Column<int>(type: "int", nullable: false),
                    protocolo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.cuentaID);
                    table.ForeignKey(
                        name: "FK_Perfiles_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SegMenAlimentacion",
                columns: table => new
                {
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    respuestaPregunta1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta9 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegMenAlimentacion", x => x.seguimientoMensualID);
                    table.ForeignKey(
                        name: "FK_SegMenAlimentacion_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SegMenEjercicios",
                columns: table => new
                {
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    pregunta1 = table.Column<int>(type: "int", nullable: false),
                    pregunta2 = table.Column<int>(type: "int", nullable: false),
                    pregunta3 = table.Column<int>(type: "int", nullable: false),
                    pregunta4 = table.Column<int>(type: "int", nullable: false),
                    pregunta5 = table.Column<int>(type: "int", nullable: false),
                    pregunta6 = table.Column<int>(type: "int", nullable: false),
                    pregunta7 = table.Column<int>(type: "int", nullable: false),
                    actCaminata = table.Column<float>(type: "real", nullable: false),
                    actfModerada = table.Column<float>(type: "real", nullable: false),
                    actfVigorosa = table.Column<float>(type: "real", nullable: false),
                    totalMET = table.Column<float>(type: "real", nullable: false),
                    conductaSend = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nivelAF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegMenEjercicios", x => x.seguimientoMensualID);
                    table.ForeignKey(
                        name: "FK_SegMenEjercicios_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SegMenHabitos",
                columns: table => new
                {
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    respuestaPregunta1 = table.Column<TimeOnly>(type: "time", nullable: false),
                    respuestaPregunta2 = table.Column<int>(type: "int", nullable: false),
                    respuestaPregunta3 = table.Column<TimeOnly>(type: "time", nullable: false),
                    respuestaPregunta4 = table.Column<int>(type: "int", nullable: false),
                    respuestaPregunta5a = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5b = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5c = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5d = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5e = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5f = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5g = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5h = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5i = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta5j = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta6 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta7 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta8 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta9 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegMenHabitos", x => x.seguimientoMensualID);
                    table.ForeignKey(
                        name: "FK_SegMenHabitos_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SegMenMedicamentos",
                columns: table => new
                {
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cuentaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    año = table.Column<int>(type: "int", nullable: false),
                    respuestaPregunta1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    respuestaPregunta4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    adherenciaTratamiento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegMenMedicamentos", x => x.seguimientoMensualID);
                    table.ForeignKey(
                        name: "FK_SegMenMedicamentos_Cuentas_cuentaID",
                        column: x => x.cuentaID,
                        principalTable: "Cuentas",
                        principalColumn: "cuentaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alimentosAlmuerzo",
                columns: table => new
                {
                    alimentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    almuerzoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreAlimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alimentosAlmuerzo", x => x.alimentoID);
                    table.ForeignKey(
                        name: "FK_alimentosAlmuerzo_Almuerzos_almuerzoID",
                        column: x => x.almuerzoID,
                        principalTable: "Almuerzos",
                        principalColumn: "almuerzoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alimentosCena",
                columns: table => new
                {
                    alimentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cenaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreAlimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alimentosCena", x => x.alimentoID);
                    table.ForeignKey(
                        name: "FK_alimentosCena_Cenas_cenaID",
                        column: x => x.cenaID,
                        principalTable: "Cenas",
                        principalColumn: "cenaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alimentosColacion",
                columns: table => new
                {
                    alimentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    colacionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreAlimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alimentosColacion", x => x.alimentoID);
                    table.ForeignKey(
                        name: "FK_alimentosColacion_Colaciones_colacionID",
                        column: x => x.colacionID,
                        principalTable: "Colaciones",
                        principalColumn: "colacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alimentosComida",
                columns: table => new
                {
                    alimentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    comidaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreAlimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alimentosComida", x => x.alimentoID);
                    table.ForeignKey(
                        name: "FK_alimentosComida_Comidas_comidaID",
                        column: x => x.comidaID,
                        principalTable: "Comidas",
                        principalColumn: "comidaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alimentosDesayuno",
                columns: table => new
                {
                    alimentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    desayunoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreAlimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidadConsumida = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alimentosDesayuno", x => x.alimentoID);
                    table.ForeignKey(
                        name: "FK_alimentosDesayuno_Desayunos_desayunoID",
                        column: x => x.desayunoID,
                        principalTable: "Desayunos",
                        principalColumn: "desayunoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resultadosAlimentacion",
                columns: table => new
                {
                    resultadosID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    puntosPregunta1 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta2 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta3 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta4 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta5 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta6 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta7 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta8 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta9 = table.Column<int>(type: "int", nullable: false),
                    puntosPregunta10 = table.Column<int>(type: "int", nullable: false),
                    totalPuntos = table.Column<int>(type: "int", nullable: false),
                    clasificacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultadosAlimentacion", x => x.resultadosID);
                    table.ForeignKey(
                        name: "FK_resultadosAlimentacion_SegMenAlimentacion_seguimientoMensualID",
                        column: x => x.seguimientoMensualID,
                        principalTable: "SegMenAlimentacion",
                        principalColumn: "seguimientoMensualID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resultadosHabitos",
                columns: table => new
                {
                    resultadosID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    seguimientoMensualID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    resultadoComponente1 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente2 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente3 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente4 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente5 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente6 = table.Column<int>(type: "int", nullable: false),
                    resultadoComponente7 = table.Column<int>(type: "int", nullable: false),
                    clasificacionGlobal = table.Column<int>(type: "int", nullable: false),
                    clasificacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultadosHabitos", x => x.resultadosID);
                    table.ForeignKey(
                        name: "FK_resultadosHabitos_SegMenHabitos_seguimientoMensualID",
                        column: x => x.seguimientoMensualID,
                        principalTable: "SegMenHabitos",
                        principalColumn: "seguimientoMensualID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alimentosAlmuerzo_almuerzoID",
                table: "alimentosAlmuerzo",
                column: "almuerzoID");

            migrationBuilder.CreateIndex(
                name: "IX_alimentosCena_cenaID",
                table: "alimentosCena",
                column: "cenaID");

            migrationBuilder.CreateIndex(
                name: "IX_alimentosColacion_colacionID",
                table: "alimentosColacion",
                column: "colacionID");

            migrationBuilder.CreateIndex(
                name: "IX_alimentosComida_comidaID",
                table: "alimentosComida",
                column: "comidaID");

            migrationBuilder.CreateIndex(
                name: "IX_alimentosDesayuno_desayunoID",
                table: "alimentosDesayuno",
                column: "desayunoID");

            migrationBuilder.CreateIndex(
                name: "IX_Almuerzos_cuentaID",
                table: "Almuerzos",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Cenas_cuentaID",
                table: "Cenas",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Colaciones_cuentaID",
                table: "Colaciones",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Comidas_cuentaID",
                table: "Comidas",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Desayunos_cuentaID",
                table: "Desayunos",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_efectoSecundarios_cuentaID",
                table: "efectoSecundarios",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_cuentaID",
                table: "Ejercicios",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_habitosBebida_cuentaID",
                table: "habitosBebida",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_habitosDroga_cuentaID",
                table: "habitosDroga",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_habitosSueño_cuentaID",
                table: "habitosSueño",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_cuentaID",
                table: "Medicamentos",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_resultadosAlimentacion_seguimientoMensualID",
                table: "resultadosAlimentacion",
                column: "seguimientoMensualID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resultadosHabitos_seguimientoMensualID",
                table: "resultadosHabitos",
                column: "seguimientoMensualID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SegMenAlimentacion_cuentaID",
                table: "SegMenAlimentacion",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_SegMenEjercicios_cuentaID",
                table: "SegMenEjercicios",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_SegMenHabitos_cuentaID",
                table: "SegMenHabitos",
                column: "cuentaID");

            migrationBuilder.CreateIndex(
                name: "IX_SegMenMedicamentos_cuentaID",
                table: "SegMenMedicamentos",
                column: "cuentaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alimentosAlmuerzo");

            migrationBuilder.DropTable(
                name: "alimentosCena");

            migrationBuilder.DropTable(
                name: "alimentosColacion");

            migrationBuilder.DropTable(
                name: "alimentosComida");

            migrationBuilder.DropTable(
                name: "alimentosDesayuno");

            migrationBuilder.DropTable(
                name: "efectoSecundarios");

            migrationBuilder.DropTable(
                name: "Ejercicios");

            migrationBuilder.DropTable(
                name: "habitosBebida");

            migrationBuilder.DropTable(
                name: "habitosDroga");

            migrationBuilder.DropTable(
                name: "habitosSueño");

            migrationBuilder.DropTable(
                name: "Medicamentos");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "resultadosAlimentacion");

            migrationBuilder.DropTable(
                name: "resultadosHabitos");

            migrationBuilder.DropTable(
                name: "SegMenEjercicios");

            migrationBuilder.DropTable(
                name: "SegMenMedicamentos");

            migrationBuilder.DropTable(
                name: "Almuerzos");

            migrationBuilder.DropTable(
                name: "Cenas");

            migrationBuilder.DropTable(
                name: "Colaciones");

            migrationBuilder.DropTable(
                name: "Comidas");

            migrationBuilder.DropTable(
                name: "Desayunos");

            migrationBuilder.DropTable(
                name: "SegMenAlimentacion");

            migrationBuilder.DropTable(
                name: "SegMenHabitos");

            migrationBuilder.DropTable(
                name: "Cuentas");
        }
    }
}

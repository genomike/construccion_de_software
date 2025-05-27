using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtapaDeJuicio.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EtapaDeJuicio");

            migrationBuilder.CreateTable(
                name: "Audiencias",
                schema: "EtapaDeJuicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaHoraProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false, defaultValue: 120),
                    Estado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaHoraFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MotivoSuspension = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActasAudiencia",
                schema: "EtapaDeJuicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudienciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActasAudiencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActasAudiencia_Audiencias_AudienciaId",
                        column: x => x.AudienciaId,
                        principalSchema: "EtapaDeJuicio",
                        principalTable: "Audiencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActividadesAudiencia",
                schema: "EtapaDeJuicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudienciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActaAudienciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesAudiencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesAudiencia_ActasAudiencia_ActaAudienciaId",
                        column: x => x.ActaAudienciaId,
                        principalSchema: "EtapaDeJuicio",
                        principalTable: "ActasAudiencia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActividadesAudiencia_Audiencias_AudienciaId",
                        column: x => x.AudienciaId,
                        principalSchema: "EtapaDeJuicio",
                        principalTable: "Audiencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantesAudiencia",
                schema: "EtapaDeJuicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudienciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActaAudienciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantesAudiencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantesAudiencia_ActasAudiencia_ActaAudienciaId",
                        column: x => x.ActaAudienciaId,
                        principalSchema: "EtapaDeJuicio",
                        principalTable: "ActasAudiencia",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ParticipantesAudiencia_Audiencias_AudienciaId",
                        column: x => x.AudienciaId,
                        principalSchema: "EtapaDeJuicio",
                        principalTable: "Audiencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActasAudiencia_AudienciaId",
                schema: "EtapaDeJuicio",
                table: "ActasAudiencia",
                column: "AudienciaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActasAudiencia_FechaGeneracion",
                schema: "EtapaDeJuicio",
                table: "ActasAudiencia",
                column: "FechaGeneracion");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAudiencia_ActaAudienciaId",
                schema: "EtapaDeJuicio",
                table: "ActividadesAudiencia",
                column: "ActaAudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAudiencia_AudienciaId",
                schema: "EtapaDeJuicio",
                table: "ActividadesAudiencia",
                column: "AudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAudiencia_FechaHora",
                schema: "EtapaDeJuicio",
                table: "ActividadesAudiencia",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAudiencia_Tipo",
                schema: "EtapaDeJuicio",
                table: "ActividadesAudiencia",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Audiencias_Estado",
                schema: "EtapaDeJuicio",
                table: "Audiencias",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Audiencias_FechaHoraProgramada",
                schema: "EtapaDeJuicio",
                table: "Audiencias",
                column: "FechaHoraProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_Audiencias_Tipo",
                schema: "EtapaDeJuicio",
                table: "Audiencias",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesAudiencia_ActaAudienciaId",
                schema: "EtapaDeJuicio",
                table: "ParticipantesAudiencia",
                column: "ActaAudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesAudiencia_AudienciaId",
                schema: "EtapaDeJuicio",
                table: "ParticipantesAudiencia",
                column: "AudienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesAudiencia_Rol",
                schema: "EtapaDeJuicio",
                table: "ParticipantesAudiencia",
                column: "Rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActividadesAudiencia",
                schema: "EtapaDeJuicio");

            migrationBuilder.DropTable(
                name: "ParticipantesAudiencia",
                schema: "EtapaDeJuicio");

            migrationBuilder.DropTable(
                name: "ActasAudiencia",
                schema: "EtapaDeJuicio");

            migrationBuilder.DropTable(
                name: "Audiencias",
                schema: "EtapaDeJuicio");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherData.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirportStatus",
                columns: table => new
                {
                    codigo_icao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pressao_atmosferica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    visibilidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vento = table.Column<int>(type: "int", nullable: false),
                    direcao_vento = table.Column<int>(type: "int", nullable: false),
                    umidade = table.Column<int>(type: "int", nullable: false),
                    condicao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    condicao_Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    temp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportStatus", x => x.codigo_icao);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExceptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecast",
                columns: table => new
                {
                    codCidade = table.Column<int>(type: "int", nullable: false),
                    cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    atualizado_em = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecast", x => x.codCidade);
                });

            migrationBuilder.CreateTable(
                name: "DayWeather",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    condicao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    min = table.Column<int>(type: "int", nullable: false),
                    max = table.Column<int>(type: "int", nullable: false),
                    indice_uv = table.Column<int>(type: "int", nullable: false),
                    condicao_desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeatherForecastcodCidade = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayWeather", x => x.id);
                    table.ForeignKey(
                        name: "FK_DayWeather_WeatherForecast_WeatherForecastcodCidade",
                        column: x => x.WeatherForecastcodCidade,
                        principalTable: "WeatherForecast",
                        principalColumn: "codCidade");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayWeather_WeatherForecastcodCidade",
                table: "DayWeather",
                column: "WeatherForecastcodCidade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirportStatus");

            migrationBuilder.DropTable(
                name: "DayWeather");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "WeatherForecast");
        }
    }
}

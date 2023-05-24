using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastroApp.Migrations
{
    public partial class AddSamochodToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Samochody",
                columns: table => new
                {
                    IdSamochodu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NrRejestracyjny = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdRodzajuPaliwa = table.Column<int>(type: "int", nullable: false),
                    OC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Przeglad = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samochody", x => x.IdSamochodu);
                    table.ForeignKey(
                        name: "FK_Samochody_RodzajPaliw_IdRodzajuPaliwa",
                        column: x => x.IdRodzajuPaliwa,
                        principalTable: "RodzajPaliw",
                        principalColumn: "IdRodzajuPaliwa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samochody_IdRodzajuPaliwa",
                table: "Samochody",
                column: "IdRodzajuPaliwa");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Samochody");
        }
    }
}

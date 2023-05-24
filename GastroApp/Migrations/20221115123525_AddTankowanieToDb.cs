using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastroApp.Migrations
{
    public partial class AddTankowanieToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tankowania",
                columns: table => new
                {
                    IdTankowania = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSamochodu = table.Column<int>(type: "int", nullable: false),
                    IdRodzajuPaliwa = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cena = table.Column<decimal>(type: "money", nullable: false),
                    Ilosc = table.Column<float>(type: "real", nullable: false),
                    Wartosc = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tankowania", x => x.IdTankowania);
                    table.ForeignKey(
                        name: "FK_Tankowania_RodzajPaliw_IdRodzajuPaliwa",
                        column: x => x.IdRodzajuPaliwa,
                        principalTable: "RodzajPaliw",
                        principalColumn: "IdRodzajuPaliwa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tankowania_Samochody_IdSamochodu",
                        column: x => x.IdSamochodu,
                        principalTable: "Samochody",
                        principalColumn: "IdSamochodu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tankowania_IdRodzajuPaliwa",
                table: "Tankowania",
                column: "IdRodzajuPaliwa");

            migrationBuilder.CreateIndex(
                name: "IX_Tankowania_IdSamochodu",
                table: "Tankowania",
                column: "IdSamochodu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tankowania");
        }
    }
}

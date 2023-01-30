using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastroApp.Migrations
{
    public partial class AddZamowienieToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zamowienia",
                columns: table => new
                {
                    IdZamowienia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdKlienta = table.Column<int>(type: "int", nullable: false),
                    IdPracownika = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataOne = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataTwo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NrZamowienia = table.Column<int>(type: "int", nullable: true),
                    IdSposobuPlatnosci = table.Column<int>(type: "int", nullable: false),
                    IdStatusu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zamowienia", x => x.IdZamowienia);
                    table.ForeignKey(
                        name: "FK_Zamowienia_AspNetUsers_IdPracownika",
                        column: x => x.IdPracownika,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zamowienia_Klienci_IdKlienta",
                        column: x => x.IdKlienta,
                        principalTable: "Klienci",
                        principalColumn: "IdKlienta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zamowienia_SposobPlatnosci_IdSposobuPlatnosci",
                        column: x => x.IdSposobuPlatnosci,
                        principalTable: "SposobPlatnosci",
                        principalColumn: "IdSposobuPlatnosci",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zamowienia_Status_IdStatusu",
                        column: x => x.IdStatusu,
                        principalTable: "Status",
                        principalColumn: "IdStatusu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zamowienia_IdKlienta",
                table: "Zamowienia",
                column: "IdKlienta");

            migrationBuilder.CreateIndex(
                name: "IX_Zamowienia_IdPracownika",
                table: "Zamowienia",
                column: "IdPracownika");

            migrationBuilder.CreateIndex(
                name: "IX_Zamowienia_IdSposobuPlatnosci",
                table: "Zamowienia",
                column: "IdSposobuPlatnosci");

            migrationBuilder.CreateIndex(
                name: "IX_Zamowienia_IdStatusu",
                table: "Zamowienia",
                column: "IdStatusu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zamowienia");
        }
    }
}

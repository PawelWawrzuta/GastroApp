using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastroApp.Migrations
{
    public partial class AddZuzycieToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zuzycia",
                columns: table => new
                {
                    IdZuzycia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPracownika = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdSamochodu = table.Column<int>(type: "int", nullable: false),
                    Przebieg = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zuzycia", x => x.IdZuzycia);
                    table.ForeignKey(
                        name: "FK_Zuzycia_AspNetUsers_IdPracownika",
                        column: x => x.IdPracownika,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zuzycia_Samochody_IdSamochodu",
                        column: x => x.IdSamochodu,
                        principalTable: "Samochody",
                        principalColumn: "IdSamochodu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zuzycia_IdPracownika",
                table: "Zuzycia",
                column: "IdPracownika");

            migrationBuilder.CreateIndex(
                name: "IX_Zuzycia_IdSamochodu",
                table: "Zuzycia",
                column: "IdSamochodu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zuzycia");
        }
    }
}

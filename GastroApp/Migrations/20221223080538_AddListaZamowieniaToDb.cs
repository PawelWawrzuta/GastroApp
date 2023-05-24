using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastroApp.Migrations
{
    public partial class AddListaZamowieniaToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListaZamowienia",
                columns: table => new
                {
                    IdListyZamowienia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdZamowienia = table.Column<int>(type: "int", nullable: false),
                    IdProduktu = table.Column<int>(type: "int", nullable: false),
                    Cena = table.Column<decimal>(type: "money", nullable: false),
                    Ilosc = table.Column<float>(type: "real", nullable: false),
                    Wartosc = table.Column<decimal>(type: "money", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaZamowienia", x => x.IdListyZamowienia);
                    table.ForeignKey(
                        name: "FK_ListaZamowienia_Produkty_IdProduktu",
                        column: x => x.IdProduktu,
                        principalTable: "Produkty",
                        principalColumn: "IdProduktu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListaZamowienia_Zamowienia_IdZamowienia",
                        column: x => x.IdZamowienia,
                        principalTable: "Zamowienia",
                        principalColumn: "IdZamowienia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListaZamowienia_IdProduktu",
                table: "ListaZamowienia",
                column: "IdProduktu");

            migrationBuilder.CreateIndex(
                name: "IX_ListaZamowienia_IdZamowienia",
                table: "ListaZamowienia",
                column: "IdZamowienia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListaZamowienia");
        }
    }
}

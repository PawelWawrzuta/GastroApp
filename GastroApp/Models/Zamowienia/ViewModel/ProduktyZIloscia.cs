namespace GastroApp.Models.Zamowienia.ViewModel
{
    public class ProduktyZIloscia:Produkt
    {
        public ProduktyZIloscia()
        {

        }
        public ProduktyZIloscia(Produkt produkt)
        {
            Nazwa = produkt.Nazwa;
            Cena = produkt.Cena;
            IdProduktu = produkt.IdProduktu;
        }
        public float Ilosc { get; set; }
        public string IloscString { get; set; }
    }
}

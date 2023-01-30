namespace GastroApp.Models.Zamowienia.ViewModel
{
    public class ListaZamowieniaZNazwa : ListaZamowienia
    {
        public ListaZamowieniaZNazwa()
        {

        }
        public ListaZamowieniaZNazwa(ListaZamowienia listaZamowienia, string nazwa)
        {
            IdProduktu = listaZamowienia.IdProduktu;
            Cena = listaZamowienia.Cena;
            Ilosc = listaZamowienia.Ilosc;
            Opis = listaZamowienia.Opis;
            Wartosc=listaZamowienia.Wartosc;
            Nazwa = nazwa;
        }
        public string Nazwa { get; set; }
    }
}

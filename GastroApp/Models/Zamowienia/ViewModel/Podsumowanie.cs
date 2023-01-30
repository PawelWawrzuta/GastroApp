using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Zamowienia.ViewModel
{
    public class Podsumowanie : Klient
    {
        public Podsumowanie()
        {

        }
       
        public Podsumowanie(List<ListaZamowieniaZNazwa> lzn, Klient daneklienta)
        {
            ListaZamowieniaZNazwa = lzn;
            Adres = daneklienta.Adres;
            Miasto = daneklienta.Miasto;
            KodPocztowy = daneklienta.KodPocztowy;
            Telefon = daneklienta.Telefon;
        }
        public Podsumowanie(List<ListaZamowieniaZNazwa> lzn, Klient daneklienta, Zamowienie zamowienie, string nazwaSposobuPlatnosci)
        {
            ListaZamowieniaZNazwa = lzn;
            IdKlienta = daneklienta.IdKlienta;
            Adres = daneklienta.Adres;
            Miasto = daneklienta.Miasto;
            KodPocztowy = daneklienta.KodPocztowy;
            Telefon = daneklienta.Telefon;
            IdZamowienia = zamowienie.IdZamowienia;
            IdSposobuPlatnosci = zamowienie.IdSposobuPlatnosci;
            NazwaSposobuPlatnosci = nazwaSposobuPlatnosci;
            NrZamowienia = zamowienie.NrZamowienia;
        }


        public SelectList? SposobPlatnosciSelect { get; set; }

        [DisplayName("Sposób płatności")]
        public int IdSposobuPlatnosci { get; set; }
        [DisplayName("Sposób płatności")]
        public string NazwaSposobuPlatnosci { get; set; }

        public List<ListaZamowieniaZNazwa> ListaZamowieniaZNazwa { get; set; }

        [DisplayName("Nr zamowienia")]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Pole Nr zamowienia może zawierać tylko cyfry.")]
        public int? NrZamowienia { get; set; }

        public int IdZamowienia { get; set; }

        [DisplayName("Status")]
        public int IdStatusu { get; set; }
        [DisplayName("Status")]
        public string NazwaStatusu { get; set; }


    }
}

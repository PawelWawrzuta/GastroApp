using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Samochody.ViewModel
{
    public class IndexTankowania : Tankowanie
    {
        public IndexTankowania()
        {

        }
        public IndexTankowania(int idTankowania, string imie, string nazwisko, DateTime data, decimal cena, float ilosc, decimal wartosc, int idSamochodu, string nazwaRodzajuPaliwa)
        {
            IdTankowania = idTankowania;
            Imie = imie;
            Nazwisko = nazwisko;
            Data = data;
            Cena = cena;
            Ilosc = ilosc;
            Wartosc = wartosc;
            IdSamochodu = idSamochodu;
            NazwaRodzajuPaliwa = nazwaRodzajuPaliwa;
        }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        [Display(Name="Rodzaj Paliwa")]
        public string NazwaRodzajuPaliwa { get; set; }
    }
}

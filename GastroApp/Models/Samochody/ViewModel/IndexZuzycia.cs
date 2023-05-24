namespace GastroApp.Models.Samochody.ViewModel
{
    public class IndexZuzycia : Zuzycie
    {
        public IndexZuzycia()
        {

        }
        public IndexZuzycia(int idZuzycia, int przebieg, DateTime data, int idSamochodu, string imie, string nazwisko)
        {
            IdZuzycia = idZuzycia;
            Przebieg = przebieg;
            Data = data;
            IdSamochodu = idSamochodu;
            Imie = imie;
            Nazwisko = nazwisko;
        }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Samochody.ViewModel
{
    public class SamochodZRodzajemPaliwa : Samochod
    {
        public SamochodZRodzajemPaliwa()
        {

        }
        public SamochodZRodzajemPaliwa(Samochod samochod, List<RodzajPaliwa> rodzajPaliwaList)
        {
            IdSamochodu = samochod.IdSamochodu;
            Marka = samochod.Marka;
            Model = samochod.Model;
            NrRejestracyjny = samochod.NrRejestracyjny;
            Opis = samochod.Opis;
            IdRodzajuPaliwa = samochod.IdRodzajuPaliwa;
            OC = samochod.OC;
            AC = samochod.AC;
            Przeglad = samochod.Przeglad;
            RodzajPaliwaList = rodzajPaliwaList;
        }
        public List<RodzajPaliwa>? RodzajPaliwaList { get; set; }
        public SelectList? RodzajPaliwaSelect { get; set; }
    }
}

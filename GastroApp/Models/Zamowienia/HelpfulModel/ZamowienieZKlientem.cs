using Microsoft.AspNetCore.Mvc.Rendering;

namespace GastroApp.Models.Zamowienia.HelpfulModel
{
    public class ZamowienieZKlientem : Zamowienie
    {
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string StatusNazwa { get; set; }
        public string SposobPlatnosciNazwa { get; set; }
        public decimal WartoscCalkowita { get; set; }

        public bool Zaznacz { get; set; }
        public string UserName { get; set; }
    }
}

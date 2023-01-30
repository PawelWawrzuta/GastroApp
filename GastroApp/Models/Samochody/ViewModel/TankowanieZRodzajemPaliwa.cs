using Microsoft.AspNetCore.Mvc.Rendering;

namespace GastroApp.Models.Samochody.ViewModel
{
    public class TankowanieZRodzajemPaliwa : Tankowanie
    {
        public SelectList? RodzajPaliwaSelect { get; set; }
    }
}

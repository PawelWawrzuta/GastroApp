using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Pracownicy.ViewModel
{
    public class PracownikIndex : Pracownik
    {
        [Display(Name ="Stanowisko")]
        public string Roles { get; set; }
    }
}


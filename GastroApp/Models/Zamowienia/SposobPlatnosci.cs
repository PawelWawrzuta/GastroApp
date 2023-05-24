using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Zamowienia
{
    public class SposobPlatnosci
    {
        [Key]
        public int IdSposobuPlatnosci { get; set; }
        public string Nazwa { get; set; }
    }
}

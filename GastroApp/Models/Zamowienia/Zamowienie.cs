using GastroApp.Models.Pracownicy;
using GastroApp.Models.Samochody;
using GastroApp.Models.Zamowienia.ViewModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models.Zamowienia
{
    public class Zamowienie
    {
        public Zamowienie()
        {

        }
        public Zamowienie(int idKlienta, Podsumowanie podsumowanie, int idStatusu)
        {
            IdKlienta = idKlienta;
            NrZamowienia = podsumowanie.NrZamowienia;
            IdSposobuPlatnosci = podsumowanie.IdSposobuPlatnosci;
            IdStatusu = idStatusu;
        }

        [Key]
        public int IdZamowienia { get; set; }

        [Required]
        public int IdKlienta { get; set; }
        [ForeignKey("IdKlienta")]
        public Klient? Klient { get; set; }

        public string? IdPracownika { get; set; }
        [ForeignKey("IdPracownika")]
        public Pracownik? Pracownik { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataOne { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataTwo { get; set; }

        [DisplayName("Nr zamowienia")]
        [RegularExpression("^[0-9]{1,}$", ErrorMessage = "Pole Nr zamowienia może zawierać tylko cyfry.")]
        public int? NrZamowienia { get; set; }

        [DisplayName("Sposób płatności")]
        [Required]
        public int IdSposobuPlatnosci { get; set; }
        [ForeignKey("IdSposobuPlatnosci")]
        public SposobPlatnosci? SposobPlatnosci { get; set; }

        [DisplayName("Status zamówienia")]
        [Required]
        public int IdStatusu { get; set; }
        [ForeignKey("IdStatusu")]
        public Status? Status { get; set; }
    }
}

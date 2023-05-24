using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Zamowienia
{
    public class Klient
    {
        [Key]
        public int IdKlienta { get; set; }

        [Required(ErrorMessage = "Pole Adres jest wymagane.")]
        [RegularExpression("^([0-9]{1,}[ ])?([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,}[ ]){1,3}([0-9]{1,})([a-z]{1,})?(/[0-9]{1,})?$", ErrorMessage = "Pole Adres musi być w formacie: ulica numerDomu/numerMieszkania")]
        [MaxLength(50)]
        public string Adres { get; set; }

        
        [RegularExpression("^(([A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,})(-[A-ZĄĆĘŁŃÓŚŹŻ][a-ząćęłńóśźż]{2,})?)*$", ErrorMessage = "Pole Miasto musi zaczynać się od wielkiej litery i może zawierać tylko litery.")]
        [MaxLength(50)]
        public string? Miasto { get; set; }

        [DisplayName("Kod pocztowy")]
        [RegularExpression("^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Pole Kod pocztowy musi być w formacie: xx-xxx")]
        [MaxLength(6)]
        public string? KodPocztowy { get; set; }

        [Required(ErrorMessage = "Pole Telefon jest wymagane.")]
        [RegularExpression("^([1-9]{1})([0-9]{2})((?:[-| ]*[0-9]){3}){2}$", ErrorMessage = "Pole Telefon musi zawierać 9 cyfr. Dostępne formaty: xxxxxxxxx, xxx xxx xxx, xxx-xxx-xxx")]
        [MaxLength(11)]
        public string Telefon { get; set; }

        public string? SzerokoscGeograficzna { get; set; }
        public string? DlugoscGeograficzna { get; set; }

    }
}

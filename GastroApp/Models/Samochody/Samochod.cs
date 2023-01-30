using GastroApp.Models.Samochody;
using GastroApp.Models.Samochody.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models.Samochody
{
    public class Samochod
    {
        public Samochod()
        {

        }
        public Samochod(SamochodZRodzajemPaliwa srp)
        {
            IdSamochodu = srp.IdSamochodu;
            Marka = srp.Marka;
            Model = srp.Model;
            NrRejestracyjny = srp.NrRejestracyjny;
            Opis = srp.Opis;
            IdRodzajuPaliwa = srp.IdRodzajuPaliwa;
            OC = srp.OC;
            AC = srp.AC;
            Przeglad = srp.Przeglad;
        }

        [Key]
        public int IdSamochodu { get; set; }

        [Required(ErrorMessage = "Pole Marka jest wymagane.")]
        [RegularExpression("^([A-Z]?[a-z]{1,}[ ]?){1,}$", ErrorMessage = "Pole Marka może zawierać tylko litery.")]
        [MaxLength(20)]
        public string Marka { get; set; }

        [Required(ErrorMessage = "Pole Model jest wymagane.")]
        [MaxLength(20)]
        public string Model { get; set; }

        [Display(Name = "Numer Rejestracyjny")]
        [Required(ErrorMessage = "Pole Numer Rejestracyjny jest wymagane.")]
        [RegularExpression("^[A-Za-z]{1}(([A-Za-z0-9]{4,6}))$", ErrorMessage = "Pole Numer Rejestracyjny musi być w formacie: 1 litera i od 4 do 6 cyfr lub liter!")]
        [MaxLength(7)]
        public string NrRejestracyjny { get; set; }


        [MaxLength(50)]
        public string? Opis { get; set; }

        [Required]
        [Display(Name = "Rodzaj Paliwa")]
        public int IdRodzajuPaliwa { get; set; }
        [ForeignKey("IdRodzajuPaliwa")]
        public RodzajPaliwa? RodzajPaliwa { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? OC { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AC { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Przeglad { get; set; }
    }
}

using GastroApp.Models.Pracownicy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models.Samochody
{
    public class Zuzycie
    {
        public Zuzycie()
        {

        }
        public Zuzycie(string idPracownika, int idSamochodu, int przebieg, DateTime data)
        {
            IdPracownika = idPracownika;
            IdSamochodu = idSamochodu;
            Przebieg = przebieg;
            Data = data;
        }

        [Key]
        public int IdZuzycia { get; set; }

        [Required]
        public string IdPracownika { get; set; }
        [ForeignKey("IdPracownika")]
        public virtual Pracownik? Pracownik { get; set; }

        [Required]
        public int IdSamochodu { get; set; }

        [ForeignKey("IdSamochodu")]
        public virtual Samochod? Samochod { get; set; }

        [Required(ErrorMessage = "Pole Przebieg jest wymagane.")]

        [RegularExpression("^[0-9]*$", ErrorMessage = "Pole Przebieg może zawierać tylko cyfry.")]
        public int Przebieg { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; } = DateTime.Now;

    }
}


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models.Samochody
{
    public class Tankowanie
    {
        public Tankowanie()
        {

        }
        public Tankowanie(int idSamochodu, int idRodzajuPaliwa, decimal cena, float ilosc)
        {
            IdSamochodu = idSamochodu;
            IdRodzajuPaliwa = idRodzajuPaliwa;
            Cena=cena;
            Ilosc = ilosc;
        }

        [Key]
        public int IdTankowania { get; set; }

        [Required]
        public int IdSamochodu { get; set; }

        [ForeignKey("IdSamochodu")]
        public virtual Samochod? Samochod { get; set; }

        [Required]
        [Display(Name = "Rodzaj Paliwa")]
        public int IdRodzajuPaliwa { get; set; }
        [ForeignKey("IdRodzajuPaliwa")]
        public virtual RodzajPaliwa? RodzajPaliwa { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Pole Cena jest wymagane.")]
        [RegularExpression("^[0-9]*([,][0-9]{2,2}?)$", ErrorMessage = "Pole Cena może zawierać tylko cyfry oraz musi być w formacie x lub x,xx")]
        [Column(TypeName = "money")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Pole Ilosc jest wymagane.")]
        [RegularExpression("^[0-9]*([,][0-9]{2,2})?$", ErrorMessage = "Pole Ilosc może zawierać tylko cyfry oraz musi być w formacie x,xx")]
        public float Ilosc { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Wartosc { get; set; }
    }
}

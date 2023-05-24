using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models
{
    public class Produkt
    {
        [Key]
        public int IdProduktu { get; set; }

        [Required(ErrorMessage ="Pole Nazwa jest wymagane.")]
        [RegularExpression("^([A-ZĄĆĘŁŃÓŚŹŻ]?[a-ząćęłńóśźż]{1,}[ ]?){1,}$", ErrorMessage = "Pole Nazwa może zawierać tylko litery.")]
        [MaxLength(50)]
        public string Nazwa  { get; set; }

        [Required(ErrorMessage = "Pole Cena jest wymagane.")]
        [RegularExpression("^[0-9]*([,][0-9]{2,2})?$", ErrorMessage = "Pole Cena może zawierać tylko cyfry oraz musi być w formacie x lub x,xx")]
        [Column(TypeName = "money")]
        public decimal Cena { get; set; }

        [MaxLength(50)]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Pole Dostępność jest wymagane.")]
        [Display(Name ="Dostępność")]
        public bool Dostepnosc { get; set; }

    }
}

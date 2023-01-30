using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroApp.Models.Zamowienia
{
    public class ListaZamowienia
    {
        public ListaZamowienia()
        {

        }
        public ListaZamowienia(int idProduktu, decimal cena, float ilosc, string opis)
        {
            IdProduktu = idProduktu;
            Cena = cena;
            Ilosc = ilosc;  
            Opis = opis;
        }

        [Key]
        public int IdListyZamowienia { get; set; }

        [Required]
        public int IdZamowienia { get; set; }
        [ForeignKey("IdZamowienia")]
        public virtual Zamowienie? Zamowienie { get; set; }

        [Required]
        public int IdProduktu { get; set; }
        [ForeignKey("IdProduktu")]
        public virtual Produkt? Produkt { get; set; }

        [Required(ErrorMessage = "Pole Cena jest wymagane.")]
        [RegularExpression("^[0-9]*([,][0-9]{2,2})?$", ErrorMessage = "Pole Cena może zawierać tylko cyfry oraz musi być w formacie x lub x,xx")]
        [Column(TypeName = "money")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Pole Ilosc jest wymagane.")]
        [RegularExpression("^[0-9]*([,][0,5]{2,2})?$", ErrorMessage = "Pole Ilosc może zawierać tylko cyfry oraz musi być w formacie x lub x,50")]
        public float Ilosc { get; set; }

        [Column(TypeName = "money")]
        public decimal Wartosc { get; set; }

        [MaxLength(50)]
        public string? Opis { get; set; }
    }
}

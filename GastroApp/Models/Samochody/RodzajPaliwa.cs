using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Samochody
{
    public class RodzajPaliwa
    {
        [Key]
        public int IdRodzajuPaliwa { get; set; }
        public string Nazwa { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace GastroApp.Models.Zamowienia
{
    public class Status
    {
        [Key]
        public int IdStatusu { get; set; }
        public string Nazwa { get; set; }
    }
}
